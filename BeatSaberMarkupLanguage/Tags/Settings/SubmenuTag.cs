﻿using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Settings;
using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SubmenuTag : BSMLTag
    {
        private ModSettingsFlowCoordinator modSettingsFlowCoordinator;

        public override string[] Aliases => new[] { "settings-submenu" };

        public override void Setup()
        {
            base.Setup();
            modSettingsFlowCoordinator = DiContainer.Resolve<ModSettingsFlowCoordinator>();
        }

        public override GameObject CreateObject(Transform parent)
        {
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

            LocalizableText localizedText = CreateLocalizableText(gameObj);

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
