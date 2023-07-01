﻿using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class GenericSliderSettingTag<T> : BSMLTag
        where T : GenericSliderSetting
    {
        private LayoutElement controllersTransformTemplate;

        public override void Setup()
        {
            base.Setup();
            controllersTransformTemplate = DiContainer.Resolve<SettingsNavigationController>().GetComponentOnChild<LayoutElement>("ControllersTransformSettings/Content/PositionX");
        }

        public override GameObject CreateObject(Transform parent)
        {
            LayoutElement baseSetting = Object.Instantiate(controllersTransformTemplate, parent, false);
            baseSetting.name = "BSMLSliderSetting";

            GameObject gameObject = baseSetting.gameObject;

            T sliderSetting = gameObject.AddComponent<T>();

            sliderSetting.slider = baseSetting.GetComponentInChildren<CustomFormatRangeValuesSlider>();
            sliderSetting.slider.name = "BSMLSlider";
            sliderSetting.slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            sliderSetting.slider._enableDragging = true;

            RectTransform rectTransform = (RectTransform)sliderSetting.slider.transform;
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(52, 0);
            rectTransform.pivot = new Vector2(1, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0, 0);

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
