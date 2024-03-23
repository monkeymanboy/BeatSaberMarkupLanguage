using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModifierTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "modifier" };

        protected override PrefabParams CreatePrefab()
        {
            GameplayModifierToggle toggleTemplate = BeatSaberUI.DiContainer.Resolve<GameplaySetupViewController>()._gameplayModifiersPanelController.GetComponentsInChildren<GameplayModifierToggle>().First(gmt => gmt.name == "InstaFail");
            GameplayModifierToggle baseModifier = Object.Instantiate(toggleTemplate);
            baseModifier.name = "BSMLModifier";

            GameObject gameObject = baseModifier.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseModifier);
            Object.Destroy(gameObject.GetComponent<HoverTextSetter>());
            Object.Destroy(gameObject.transform.Find("Multiplier").gameObject);

            GameObject nameText = gameObject.transform.Find("Name").gameObject;
            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(nameText);
            localizedText.maintainTextAlignment = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);
            externalComponents.Add(gameObject.transform.Find("Icon").GetComponent<Image>());

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();
            toggleSetting.toggle = gameObject.GetComponent<Toggle>();
            toggleSetting.toggle.onValueChanged.RemoveAllListeners();

            return new PrefabParams(gameObject);
        }
    }
}
