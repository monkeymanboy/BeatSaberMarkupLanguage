using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using Polyglot;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class BSMLSettings : PersistentSingleton<BSMLSettings>, IInitializable, ILateDisposable
    {
        public List<CustomCellInfo> settingsMenus = new();

        private MainFlowCoordinator _mainFlowCoordinator;
        private ModSettingsFlowCoordinator _modSettingsFlowCoordinator;

        private bool _isInitialized;
        private Button _button;
        private Sprite _normal;
        private Sprite _hover;

        [UIValue("thumbstick-value")]
        private bool ThumbstickValue
        {
            get => Plugin.config.DisableThumbstickScroll;
            set
            {
                Plugin.config.DisableThumbstickScroll = value;
            }
        }

        public void AddSettingsMenu(string name, string resource, object host)
        {
            if (settingsMenus.Any(x => x.text == name))
            {
                return;
            }

            if (settingsMenus.Count == 0)
            {
                settingsMenus.Add(new SettingsMenu("BSML", "BeatSaberMarkupLanguage.Views.settings-about.bsml", this, Assembly.GetExecutingAssembly()));
            }

            SettingsMenu settingsMenu = new(name, resource, host, Assembly.GetCallingAssembly());
            settingsMenus.Add(settingsMenu);

            if (_isInitialized)
            {
                settingsMenu.Setup();
            }

            if (_button != null)
            {
                _button.gameObject.SetActive(true);
            }
        }

        public void RemoveSettingsMenu(object host)
        {
            IEnumerable<CustomCellInfo> menu = settingsMenus.Where(x => (x as SettingsMenu).host == host);
            if (menu.Count() > 0)
            {
                settingsMenus.Remove(menu.FirstOrDefault());
            }
        }

        public void Initialize()
        {
            foreach (SettingsMenu menu in settingsMenus)
            {
                menu.didSetup = false;
            }

            AddButtonToMainScreen().ContinueWith(task => Logger.Log.Error($"Failed to add button to main screen\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);

            _isInitialized = true;
        }

        public void LateDispose()
        {
            _mainFlowCoordinator = null;
            _modSettingsFlowCoordinator = null;
        }

        [UIAction("set-thumbstick")]
        private void SetThumbstick(bool value)
        {
            ThumbstickValue = value;
        }

        [Inject]
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Zenject")]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, ModSettingsFlowCoordinator flowCoordinator)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _modSettingsFlowCoordinator = flowCoordinator;
        }

        private async Task AddButtonToMainScreen()
        {
            OptionsViewController optionsViewController = BeatSaberUI.DiContainer.Resolve<OptionsViewController>();
            _button = UnityEngine.Object.Instantiate(optionsViewController._settingsButton, optionsViewController.transform.Find("Wrapper"));
            _button.GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = "Mod Settings";
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(PresentSettings);

            if (settingsMenus.Count == 0)
            {
                _button.gameObject.SetActive(false);
            }

            _normal = await Utilities.LoadSpriteFromAssemblyAsync("BeatSaberMarkupLanguage.Resources.mods_idle.png");
            _normal.texture.wrapMode = TextureWrapMode.Clamp;

            _hover = await Utilities.LoadSpriteFromAssemblyAsync("BeatSaberMarkupLanguage.Resources.mods_selected.png");
            _hover.texture.wrapMode = TextureWrapMode.Clamp;

            _button.SetButtonStates(_normal, _hover);
        }

        private void PresentSettings()
        {
            _modSettingsFlowCoordinator.isAnimating = true;
            _mainFlowCoordinator.PresentFlowCoordinator(
                _modSettingsFlowCoordinator,
                new Action(() =>
                {
                    _modSettingsFlowCoordinator.ShowInitial();
                    _modSettingsFlowCoordinator.isAnimating = false;
                }),
                ViewController.AnimationDirection.Vertical);
        }
    }
}
