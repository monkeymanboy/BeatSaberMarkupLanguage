using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Util;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class BSMLSettings : ZenjectSingleton<BSMLSettings>, IInitializable
    {
        private readonly OptionsViewController optionsViewController;
        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly ModSettingsFlowCoordinator modSettingsFlowCoordinator;

        private readonly SettingsMenu settingsMenu;

        private bool isInitialized;
        private Button button;
        private Sprite normal;
        private Sprite hover;

        [Inject]
        private BSMLSettings(OptionsViewController optionsViewController, MainFlowCoordinator mainFlowCoordinator, ModSettingsFlowCoordinator modSettingsFlowCoordinator)
        {
            SettingsMenus = new SortedList<CustomCellInfo>(Comparer<CustomCellInfo>.Create(CompareSettingsMenu));

            this.settingsMenu = new SettingsMenu("BSML", "BeatSaberMarkupLanguage.Views.settings-about.bsml", this, Assembly.GetExecutingAssembly());
            SettingsMenus.Add(this.settingsMenu);

            this.optionsViewController = optionsViewController;
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.modSettingsFlowCoordinator = modSettingsFlowCoordinator;
        }

        internal IList<CustomCellInfo> SettingsMenus { get; }

        [UIValue("thumbstick-value")]
        private bool ThumbstickValue
        {
            get => Plugin.Config.DisableThumbstickScroll;
            set
            {
                Plugin.Config.DisableThumbstickScroll = value;
            }
        }

        public void AddSettingsMenu(string name, string resource, object host)
        {
            if (SettingsMenus.Any(x => x.Text == name))
            {
                return;
            }

            SettingsMenu settingsMenu = new(name, resource, host, Assembly.GetCallingAssembly());
            SettingsMenus.Add(settingsMenu);

            if (isInitialized)
            {
                settingsMenu.Setup();
            }

            if (button != null)
            {
                button.gameObject.SetActive(true);
            }
        }

        public void RemoveSettingsMenu(object host)
        {
            CustomCellInfo menu = SettingsMenus.Cast<SettingsMenu>().Where(x => x.Host == host).FirstOrDefault();
            if (menu != null)
            {
                SettingsMenus.Remove(menu);
            }
        }

        public void Initialize()
        {
            AddButtonToMainScreen().ContinueWith(task => Logger.Log.Error($"Failed to add button to main screen\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);

            foreach (SettingsMenu settingsMenu in SettingsMenus)
            {
                settingsMenu.Setup();
            }

            isInitialized = true;
        }

        private int CompareSettingsMenu(CustomCellInfo a, CustomCellInfo b)
        {
            // BSML's menu should always be at the top
            if (a == settingsMenu)
            {
                return int.MinValue;
            }
            else if (b == settingsMenu)
            {
                return int.MaxValue;
            }
            else
            {
                return Utilities.StripHtmlTags(a.Text).CompareTo(Utilities.StripHtmlTags(b.Text));
            }
        }

        [UIAction("set-thumbstick")]
        private void SetThumbstick(bool value)
        {
            ThumbstickValue = value;
        }

        private async Task AddButtonToMainScreen()
        {
            button = Object.Instantiate(optionsViewController._settingsButton, optionsViewController.transform.Find("Wrapper"));
            button.GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = "BSML_MOD_SETTINGS_BUTTON";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PresentSettings);

            if (SettingsMenus.Count == 0)
            {
                button.gameObject.SetActive(false);
            }

            ButtonSpriteSwap swap = button.GetComponent<ButtonSpriteSwap>();

            normal = await Utilities.LoadSpriteFromAssemblyAsync("BeatSaberMarkupLanguage.Resources.mods_idle.png");
            normal.texture.wrapMode = TextureWrapMode.Clamp;

            hover = await Utilities.LoadSpriteFromAssemblyAsync("BeatSaberMarkupLanguage.Resources.mods_selected.png");
            hover.texture.wrapMode = TextureWrapMode.Clamp;

            swap._disabledStateSprite = normal;
            swap._normalStateSprite = normal;
            swap._highlightStateSprite = hover;
            swap._pressedStateSprite = hover;
        }

        private void PresentSettings()
        {
            modSettingsFlowCoordinator.IsAnimating = true;
            mainFlowCoordinator.PresentFlowCoordinator(
                modSettingsFlowCoordinator,
                () =>
                {
                    modSettingsFlowCoordinator.ShowInitial();
                    modSettingsFlowCoordinator.IsAnimating = false;
                },
                ViewController.AnimationDirection.Vertical);
        }
    }
}
