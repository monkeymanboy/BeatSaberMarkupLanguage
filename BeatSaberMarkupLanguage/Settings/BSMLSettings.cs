using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using Polyglot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class BSMLSettings : MonoBehaviour
    {
        private bool isInitialized;
        private Button button;
        private Sprite normal;
        private Sprite hover;
        private static BSMLSettings _instance = null;

        private ModSettingsFlowCoordinator flowCoordinator;

        public List<CustomCellInfo> settingsMenus = new List<CustomCellInfo>();

        public static BSMLSettings instance
        {
            get
            {
                if (!_instance)
                    _instance = new GameObject("BSMLSettings").AddComponent<BSMLSettings>();

                return _instance;
            }
            private set => _instance = value;
        }

        internal void Setup()
        {
            foreach (SettingsMenu menu in settingsMenus)
                menu.didSetup = false;

            StopAllCoroutines();
            if (button == null)
                StartCoroutine(AddButtonToMainScreen());

            isInitialized = true;
        }

        private void Awake() => DontDestroyOnLoad(this.gameObject);

        public void AddSettingsMenu(string name, string resource, object host)
        {
            if (settingsMenus.Any(x => x.text == name))
                return;

            if (settingsMenus.Count == 0)
            {
                settingsMenus.Add(new SettingsMenu("BSML", "BeatSaberMarkupLanguage.Views.settings-about.bsml", this, Assembly.GetExecutingAssembly()));
            }
            SettingsMenu settingsMenu = new SettingsMenu(name, resource, host, Assembly.GetCallingAssembly());
            settingsMenus.Add(settingsMenu);
            if (isInitialized)
                settingsMenu.Setup();
            if (button != null)
                button.gameObject.SetActive(true);
        }

        public void RemoveSettingsMenu(object host)
        {
            IEnumerable<CustomCellInfo> menu = settingsMenus.Where(x => (x as SettingsMenu).host == host);
            if (menu.Count() > 0)
                settingsMenus.Remove(menu.FirstOrDefault());
        }

        private IEnumerator AddButtonToMainScreen()
        {
            OptionsViewController optionsViewController = null;
            while (optionsViewController == null)
            {
                optionsViewController = Resources.FindObjectsOfTypeAll<OptionsViewController>().FirstOrDefault();
                yield return new WaitForFixedUpdate();
            }
            button = Instantiate(optionsViewController._settingsButton, optionsViewController.transform.Find("Wrapper"));
            button.GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = "Mod Settings";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PresentSettings);

            if (settingsMenus.Count == 0)
                button.gameObject.SetActive(false);

            normal = Utilities.FindSpriteInAssembly("BSML:BeatSaberMarkupLanguage.Resources.mods_idle.png");
            normal.texture.wrapMode = TextureWrapMode.Clamp;

            hover = Utilities.FindSpriteInAssembly("BSML:BeatSaberMarkupLanguage.Resources.mods_selected.png");
            hover.texture.wrapMode = TextureWrapMode.Clamp;

            button.SetButtonStates(normal, hover);
        }

        private void PresentSettings()
        {
            if (flowCoordinator == null)
                flowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModSettingsFlowCoordinator>();
            flowCoordinator.isAnimating = true;
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, new Action(() =>
            {
                flowCoordinator.ShowInitial();
                flowCoordinator.isAnimating = false;
            }), ViewController.AnimationDirection.Vertical);
        }


        [UIValue("thumbstick_value")]
        private bool Thumbstick_Value
        {
            get => Plugin.config.DisableThumbstickScroll;
            set
            {
                Plugin.config.DisableThumbstickScroll = value;
            }
        }

        [UIAction("set_thumbstick")]
        private void Set_Thumbstick(bool value)
        {
            Thumbstick_Value = value;
        }
    }
}
