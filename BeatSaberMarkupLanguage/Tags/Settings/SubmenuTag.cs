using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SubmenuTag : BSMLTag
    {
        private ModSettingsFlowCoordinator modSettingsFlowCoordinator;

        public override string[] Aliases => new[] { "settings-submenu" };

        public override GameObject CreateObject(Transform parent)
        {
            modSettingsFlowCoordinator = DiContainer.Resolve<ModSettingsFlowCoordinator>();

            GameObject gameObj = new("BSMLSubmenu")
            {
                layer = 5,
            };

            gameObj.SetActive(false);

            ClickableText clickableText = gameObj.AddComponent<ClickableText>();
            clickableText.font = BeatSaberUI.MainTextFont;
            clickableText.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            clickableText.rectTransform.SetParent(parent, false);
            clickableText.text = "Default Text";
            clickableText.fontSize = 4;
            clickableText.fontStyle = FontStyles.Italic;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(gameObj);

            ViewController submenuController = BeatSaberUI.CreateViewController<ViewController>();
            SettingsMenu.SetupViewControllerTransform(submenuController);

            clickableText.OnClickEvent += (eventData) =>
            {
                if (modSettingsFlowCoordinator != null)
                {
                    modSettingsFlowCoordinator.OpenMenu(submenuController, true, false);
                }
            };

            ExternalComponents externalComponents = submenuController.gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(clickableText);
            externalComponents.components.Add(clickableText.rectTransform);
            externalComponents.components.Add(localizedText);

            gameObj.SetActive(true);
            return submenuController.gameObject;
        }
    }
}
