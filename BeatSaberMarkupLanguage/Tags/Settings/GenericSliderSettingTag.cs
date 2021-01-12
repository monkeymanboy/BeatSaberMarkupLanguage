using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class GenericSliderSettingTag<T> : BSMLTag where T : GenericSliderSetting
    {
        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => (x.name == "VRRenderingScale")), parent, false);
            baseSetting.name = "BSMLSliderSetting";

            GameObject gameObject = baseSetting.gameObject;

            T sliderSetting = gameObject.AddComponent<T>();
            GameObject.Destroy(gameObject.transform.Find("ValuePicker").gameObject);
            sliderSetting.slider = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.TimeSlider>().First(s => s.name == "RangeValuesTextSlider" && s.transform.parent?.name == "SongStart"), gameObject.transform, false);
            sliderSetting.slider.name = "BSMLSlider";
            sliderSetting.slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (sliderSetting.slider.transform as RectTransform).anchorMin = new Vector2(1, 0);
            (sliderSetting.slider.transform as RectTransform).anchorMax = new Vector2(1, 1);
            (sliderSetting.slider.transform as RectTransform).sizeDelta = new Vector2(40, 0);
            (sliderSetting.slider.transform as RectTransform).pivot = new Vector2(1, 0.5f);
            (sliderSetting.slider.transform as RectTransform).anchoredPosition = new Vector2(0, 0);

            MonoBehaviour.Destroy(baseSetting);

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
