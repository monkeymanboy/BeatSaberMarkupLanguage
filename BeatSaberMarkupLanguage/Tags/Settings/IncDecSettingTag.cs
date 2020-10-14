using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class IncDecSettingTag<T> : BSMLTag where T : IncDecSetting
    {
        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => (x.name == "VRRenderingScale")), parent, false);
            baseSetting.name = "BSMLIncDecSetting";

            GameObject gameObject = baseSetting.gameObject;
            MonoBehaviour.Destroy(baseSetting);
            gameObject.SetActive(false);

            T boolSetting = gameObject.AddComponent<T>();
            boolSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.text.richText = true;
            boolSetting.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            (gameObject.transform.GetChild(1) as RectTransform).sizeDelta = new Vector2(40, 0);
            boolSetting.text.overflowMode = TextOverflowModes.Ellipsis;

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Default Text";
            text.richText = true;
            gameObject.AddComponent<ExternalComponents>().components.Add(text);
            MonoBehaviour.Destroy(text.GetComponent<LocalizedTextMeshProUGUI>());

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
