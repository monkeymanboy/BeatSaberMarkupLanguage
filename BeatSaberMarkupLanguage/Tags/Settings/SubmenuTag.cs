using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using VRUI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SubmenuTag : BSMLTag
    {
        public override string[] Aliases => new[] { "settings-submenu"};

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObj = new GameObject("BSMLSubmenu");
            gameObj.SetActive(false);

            ClickableText clickableText = gameObj.AddComponent<ClickableText>();
            clickableText.font = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            clickableText.rectTransform.SetParent(parent, false);
            clickableText.text = "Default Text";
            clickableText.fontSize = 5;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);
            
            VRUIViewController submenuController = BeatSaberUI.CreateViewController<VRUIViewController>();
            BSMLSettings.SetupViewControllerTransform(submenuController);

            clickableText.OnClickEvent += delegate {
                ModSettingsFlowCoordinator settingsFlowCoordinator = Resources.FindObjectsOfTypeAll<ModSettingsFlowCoordinator>().FirstOrDefault();
                if (settingsFlowCoordinator)
                {
                    settingsFlowCoordinator.OpenMenu(submenuController, true, false);
                }
            };
            submenuController.gameObject.AddComponent<SubmenuText>().submenuText = clickableText;

            gameObj.SetActive(true);
            return submenuController.gameObject;
        }
    }
}
