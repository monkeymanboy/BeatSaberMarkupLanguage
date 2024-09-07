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
    public class ModifierTag : BSMLTag
    {
        private GameplayModifierToggle toggleTemplate;

        public override string[] Aliases => new[] { "modifier" };

        public override GameObject CreateObject(Transform parent)
        {
            if (toggleTemplate == null)
            {
                toggleTemplate = DiContainer.Resolve<GameplaySetupViewController>()._gameplayModifiersPanelController.GetComponentsInChildren<GameplayModifierToggle>().First(gmt => gmt.name == "InstaFail");
            }

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

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(nameText);
            localizedText.maintainTextAlignment = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().Components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);
            externalComponents.Add(gameObject.transform.Find("Icon").GetComponent<Image>());

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();
            toggleSetting.Toggle = gameObject.GetComponent<Toggle>();
            toggleSetting.Toggle.onValueChanged.RemoveAllListeners();

            gameObject.SetActive(true);

            return gameObject;
        }
    }
}
