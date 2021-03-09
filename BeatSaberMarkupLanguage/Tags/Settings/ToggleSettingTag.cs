using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ToggleSettingTag : BSMLTag
    {
        private GameObject toggleTemplate;

        public override string[] Aliases => new[] { "toggle-setting", "bool-setting", "checkbox-setting", "checkbox" };
        public virtual string PrefabToggleName => "Fullscreen";

        public override GameObject CreateObject(Transform parent)
        {
            if (toggleTemplate == null)
                toggleTemplate = Resources.FindObjectsOfTypeAll<Toggle>().Select(x => x.transform.parent.gameObject).First(p => p.name == PrefabToggleName);
            GameObject gameObject = Object.Instantiate(toggleTemplate, parent, false);
            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            Object.Destroy(gameObject.GetComponent<BoolSettingsController>());

            gameObject.name = "BSMLToggle";
            gameObject.SetActive(false);

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();

            toggleSetting.toggle = gameObject.GetComponentInChildren<Toggle>();
            toggleSetting.toggle.interactable = true;
            toggleSetting.toggle.isOn = false;
            toggleSetting.toggle.onValueChanged.RemoveAllListeners();

            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            toggleSetting.text = nameText.GetComponent<TextMeshProUGUI>();
            toggleSetting.text.text = "Default Text";
            toggleSetting.text.richText = true;
            toggleSetting.text.overflowMode = TextOverflowModes.Ellipsis;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(toggleSetting.text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);

            return gameObject;
        }
    }
}
