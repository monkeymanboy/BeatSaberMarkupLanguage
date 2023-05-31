using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class IncDecSettingTag<T> : BSMLTag
        where T : IncDecSetting
    {
        private FormattedFloatListSettingsValueController valueControllerTemplate;

        public override GameObject CreateObject(Transform parent)
        {
            if (valueControllerTemplate == null)
            {
                valueControllerTemplate = Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => x.name == "VRRenderingScale");
            }

            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
            baseSetting.name = "BSMLIncDecSetting";

            GameObject gameObject = baseSetting.gameObject;
            Object.Destroy(baseSetting);
            gameObject.SetActive(false);

            T boolSetting = gameObject.AddComponent<T>();
            boolSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.text.richText = true;
            boolSetting.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            (gameObject.transform.GetChild(1) as RectTransform).sizeDelta = new Vector2(40, 0);
            boolSetting.text.overflowMode = TextOverflowModes.Ellipsis;

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";
            text.richText = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
