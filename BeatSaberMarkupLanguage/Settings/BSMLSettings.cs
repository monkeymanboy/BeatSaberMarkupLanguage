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
    public class BSMLSettings : PersistentSingleton<BSMLSettings>
    {
        public List<CustomCellInfo> settingsMenus = new List<CustomCellInfo>();

        private ModSettingsFlowCoordinator flowCoordinator;

        [UIObject("content")]
        private GameObject content;
        
        public void AddSettingsMenu(string name, string resource, object host, bool includeAutoFormatting = true)
        {
            if (settingsMenus.Count == 0)
            {
                VRUIViewController aboutController = BeatSaberUI.CreateViewController<VRUIViewController>();
                aboutController.rectTransform.sizeDelta = new Vector2(110, 0);
                aboutController.rectTransform.anchorMin = new Vector2(0.5f, 0);
                aboutController.rectTransform.anchorMax = new Vector2(0.5f, 1);
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-container.bsml"), aboutController.gameObject, this);
                settingsMenus.Add(new SettingsMenu("About", aboutController, BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-about.bsml"), content, this)));
                StartCoroutine(AddButtonToMainScreen());
            }
            VRUIViewController viewController = BeatSaberUI.CreateViewController<VRUIViewController>();
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
            GameObject parent = viewController.gameObject;
            if (includeAutoFormatting)
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-container.bsml"), viewController.gameObject, this);
                parent = content;
            }
            settingsMenus.Add(new SettingsMenu(name, viewController, BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetCallingAssembly(), resource), parent, host)));
        }

        private IEnumerator AddButtonToMainScreen()
        {
            Transform transform = null;
            while (transform == null) {//In case some goof tries to make their settings too early
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
    }
}
