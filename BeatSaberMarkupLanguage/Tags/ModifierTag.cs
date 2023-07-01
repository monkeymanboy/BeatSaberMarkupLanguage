using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModifierTag : BSMLTag
    {
        private GameplayModifierToggle toggleTemplate;

        public override string[] Aliases => new[] { "modifier" };

        public override void Setup()
        {
            base.Setup();
            toggleTemplate = DiContainer.Resolve<SelectModifiersViewController>().GetComponentOnChild<GameplayModifierToggle>("GameplayModifiers/Modifiers/InstaFail");
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameplayModifierToggle baseModifier = Object.Instantiate(toggleTemplate, parent, false);
            baseModifier.name = "BSMLModifier";

            GameObject gameObject = baseModifier.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseModifier);
            Object.Destroy(gameObject.GetComponent<HoverTextSetter>());
            Object.Destroy(gameObject.transform.Find("Multiplier").gameObject);

            GameObject nameText = gameObject.transform.Find("Name").gameObject;
            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            LocalizableText localizedText = CreateLocalizableText(nameText);
            localizedText.MaintainTextAlignment = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);
            externalComponents.Add(gameObject.transform.Find("Icon").GetComponent<Image>());

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();
            toggleSetting.toggle = gameObject.GetComponent<Toggle>();
            toggleSetting.toggle.onValueChanged.RemoveAllListeners();

            gameObject.SetActive(true);

            return gameObject;
        }
    }
}
