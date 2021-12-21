using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using IPA.Utilities;
using Polyglot;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class GenericSliderSettingTag<T> : BSMLTag where T : GenericSliderSetting
    {
        private LayoutElement controllersTransformTemplate;

        public override GameObject CreateObject(Transform parent)
        {
            if (controllersTransformTemplate == null)
                controllersTransformTemplate = Resources.FindObjectsOfTypeAll<LayoutElement>().First(x => x.name == "PositionX");

            LayoutElement baseSetting = Object.Instantiate(controllersTransformTemplate, parent, false);
            baseSetting.name = "BSMLSliderSetting";

            GameObject gameObject = baseSetting.gameObject;

            T sliderSetting = gameObject.AddComponent<T>();

            sliderSetting.slider = baseSetting.GetComponentInChildren<CustomFormatRangeValuesSlider>();
            sliderSetting.slider.name = "BSMLSlider";
            sliderSetting.slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (sliderSetting.slider as TextSlider).SetField("_enableDragging", true);
            (sliderSetting.slider.transform as RectTransform).anchorMin = new Vector2(1, 0);
            (sliderSetting.slider.transform as RectTransform).anchorMax = new Vector2(1, 1);
            (sliderSetting.slider.transform as RectTransform).sizeDelta = new Vector2(52, 0);
            (sliderSetting.slider.transform as RectTransform).pivot = new Vector2(1, 0.5f);
            (sliderSetting.slider.transform as RectTransform).anchoredPosition = new Vector2(0, 0);

            GameObject nameText = gameObject.transform.Find("Title").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";
            text.rectTransform.anchorMax = Vector2.one;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            baseSetting.preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
