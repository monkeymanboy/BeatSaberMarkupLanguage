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
            StopAllCoroutines();
            StartCoroutine(AddButtonToMainScreen());
            foreach (SettingsMenu settingsMenu in settingsMenus)
            {
                settingsMenu.Setup();
            }
            isInitialized = true;
        }

        private void Awake() => DontDestroyOnLoad(this.gameObject);
        
        public void AddSettingsMenu(string name, string resource, object host)
        {
            if (settingsMenus.Any(x => x.text == name))
                return;

            if (settingsMenus.Count == 0)
                settingsMenus.Add(new SettingsMenu("About", "BeatSaberMarkupLanguage.Views.settings-about.bsml", this, Assembly.GetExecutingAssembly()));
            SettingsMenu settingsMenu = new SettingsMenu(name, resource, host, Assembly.GetCallingAssembly());
            settingsMenus.Add(settingsMenu);
            if(isInitialized)
                settingsMenu.Setup();
            button?.gameObject.SetActive(true);
        }

        public void RemoveSettingsMenu(object host)
        {
            var menu = settingsMenus.Where(x => (x as SettingsMenu).host == host);
            if (menu.Count() > 0)
                settingsMenus.Remove(menu.FirstOrDefault());
        }

        private IEnumerator AddButtonToMainScreen()
        {
            Transform transform = null;
            while (transform == null)
            {
                try
                {
                    transform = GameObject.Find("MainMenuViewController/BottomPanel/Buttons").transform as RectTransform;
                }
                catch { }
                yield return new WaitForFixedUpdate();
            }
            button = Instantiate(transform.GetChild(0), transform).GetComponent<Button>();
            button.transform.GetChild(0).GetChild(1).GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = "Mod Settings";
            button.onClick.AddListener(PresentSettings);
            button.transform.SetSiblingIndex(0);

            if (settingsMenus.Count == 0)
                button.gameObject.SetActive(false);
        }

        private void PresentSettings()
        {
            if (flowCoordinator == null)
                flowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModSettingsFlowCoordinator>();
            flowCoordinator.isAnimating = true;
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, new Action(delegate{
                flowCoordinator.ShowInitial();
                flowCoordinator.isAnimating = false;
            }));
        }

    }
}
