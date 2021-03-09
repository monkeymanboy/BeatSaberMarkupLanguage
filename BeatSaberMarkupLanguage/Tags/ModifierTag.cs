using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModifierTag : BSMLTag
    {
        private GameplayModifierToggle toggleTemplate;

        public override string[] Aliases => new[] { "modifier" };

        public override GameObject CreateObject(Transform parent)
        {
            if (toggleTemplate == null)
                toggleTemplate = Resources.FindObjectsOfTypeAll<GameplayModifierToggle>().First(x => x.name == "InstaFail");
            GameplayModifierToggle baseModifier = Object.Instantiate(toggleTemplate, parent, false);
            baseModifier.name = "BSMLModifier";

            GameObject gameObject = baseModifier.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseModifier);
            Object.Destroy(gameObject.GetComponent<HoverHint>());
            
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
