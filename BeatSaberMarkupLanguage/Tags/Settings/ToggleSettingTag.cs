using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ToggleSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "toggle-setting", "bool-setting", "checkbox-setting", "checkbox" };
        public virtual string PrefabToggleName => "Fullscreen";

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = Object.Instantiate(Resources.FindObjectsOfTypeAll<Toggle>().Select(x => x.transform.parent.gameObject).First(p => p.name == PrefabToggleName), parent, false);
            GameObject nameText = gameObject.transform.Find("NameText").gameObject;

            gameObject.name = "BSMLToggle";
            gameObject.SetActive(false);

            Object.Destroy(nameText.GetComponent<LocalizedTextMeshProUGUI>());

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();

            toggleSetting.toggle = gameObject.GetComponentInChildren<Toggle>();
            toggleSetting.toggle.interactable = true;
            toggleSetting.toggle.onValueChanged.RemoveAllListeners();

            toggleSetting.text = nameText.GetComponent<TextMeshProUGUI>();
            toggleSetting.text.text = "Default Text";
            toggleSetting.text.richText = true;
            toggleSetting.text.overflowMode = TextOverflowModes.Ellipsis;

            gameObject.AddComponent<ExternalComponents>().components.Add(toggleSetting.text);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);

            return gameObject;
        }
    }
}
