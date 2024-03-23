using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SubmenuTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "settings-submenu" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = base.CreateObject(parent);
            ModSettingsFlowCoordinator modSettingsFlowCoordinator = BeatSaberUI.DiContainer.Resolve<ModSettingsFlowCoordinator>();

            ViewController submenuController = BeatSaberUI.CreateViewController<ViewController>();
            SettingsMenu.SetupViewControllerTransform(submenuController);

            ClickableText clickableText = gameObject.GetComponent<ClickableText>();
            LocalizedTextMeshProUGUI localizedText = gameObject.GetComponent<LocalizedTextMeshProUGUI>();

            ExternalComponents externalComponents = submenuController.gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(clickableText);
            externalComponents.components.Add(clickableText.rectTransform);
            externalComponents.components.Add(localizedText);

            clickableText.OnClickEvent += (eventData) =>
            {
                if (modSettingsFlowCoordinator != null)
                {
                    modSettingsFlowCoordinator.OpenMenu(submenuController, true, false);
                }
            };

            return submenuController.gameObject;
        }

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObj = new("BSMLSubmenu")
            {
                layer = 5,
            };

            gameObj.SetActive(false);

            ClickableText clickableText = gameObj.AddComponent<ClickableText>();
            clickableText.font = BeatSaberUI.MainTextFont;
            clickableText.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            clickableText.text = "Default Text";
            clickableText.fontSize = 4;
            clickableText.fontStyle = FontStyles.Italic;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);

            CreateLocalizableText(gameObj);

            return new PrefabParams(gameObj);
        }
    }
}
