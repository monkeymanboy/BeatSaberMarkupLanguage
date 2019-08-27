using BeatSaberMarkupLanguage.Attributes;
using BS_Utils.Utilities;
using Polyglot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class BSMLSettings : MonoBehaviour
    {
        private static BSMLSettings _instance = null;
        public static BSMLSettings instance
        {
            get
            {
                if (!_instance)
                    _instance = new GameObject("BSMLSettings").AddComponent<BSMLSettings>();
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public List<CustomCellInfo> settingsMenus = new List<CustomCellInfo>();

        private ModSettingsFlowCoordinator flowCoordinator;

        [UIObject("content")]
        private GameObject content;
        
        public void AddSettingsMenu(string name, string resource, object host, bool includeAutoFormatting = true)
        {
            if (settingsMenus.Count == 0)
            {
                VRUIViewController aboutController = BeatSaberUI.CreateViewController<VRUIViewController>();
                SetupViewControllerTransform(aboutController);
                settingsMenus.Add(new SettingsMenu("About", aboutController, BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-about.bsml"), CreateSettingsContainer(aboutController.gameObject), this)));
            }
            VRUIViewController viewController = BeatSaberUI.CreateViewController<VRUIViewController>();
            SetupViewControllerTransform(viewController);
            GameObject parent = viewController.gameObject;
            if (includeAutoFormatting)
                parent = CreateSettingsContainer(parent);
            settingsMenus.Add(new SettingsMenu(name, viewController, BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetCallingAssembly(), resource), parent, host)));
        }

        public GameObject CreateSettingsContainer(GameObject gameObject)
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-container.bsml"), gameObject, this);
            return content;
        }

        public IEnumerator AddButtonToMainScreen()
        {
            Transform transform = null;
            while (transform == null) {
                try
                {
                    transform = GameObject.Find("MainMenuViewController/BottomPanel/Buttons").transform as RectTransform;
                }
                catch
                {

                }
                yield return new WaitForFixedUpdate();
            }
            Button button = Instantiate(transform.GetChild(0), transform).GetComponent<Button>();
            button.transform.GetChild(0).GetChild(1).GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = "Mod Settings";
            button.onClick.AddListener(PresentSettings);
            button.transform.SetSiblingIndex(0);
        }

        private void PresentSettings()
        {
            if(flowCoordinator == null)
            {
                flowCoordinator = new GameObject().AddComponent<ModSettingsFlowCoordinator>();
            }
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("PresentFlowCoordinator", new object[] {flowCoordinator, new Action(delegate{
                flowCoordinator.ShowInitial();
            }), false, false });
        }

        public static void SetupViewControllerTransform(VRUIViewController viewController)
        {
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
        }
    }
}
