using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Settings;
using HMUI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SubmenuTag : BSMLTag
    {
        public override string[] Aliases => new[] { "settings-submenu" };

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

            ViewController submenuController = BeatSaberUI.CreateViewController<ViewController>();
            BSMLSettings.SetupViewControllerTransform(submenuController);

            clickableText.OnClickEvent += delegate
            {
                ModSettingsFlowCoordinator settingsFlowCoordinator = Resources.FindObjectsOfTypeAll<ModSettingsFlowCoordinator>().FirstOrDefault();
                if (settingsFlowCoordinator)
                    settingsFlowCoordinator.OpenMenu(submenuController, true, false);
            };

            ExternalComponents externalComponents = submenuController.gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(clickableText);
            externalComponents.components.Add(clickableText.rectTransform);

            gameObj.SetActive(true);
            return submenuController.gameObject;
        }
    }
}
