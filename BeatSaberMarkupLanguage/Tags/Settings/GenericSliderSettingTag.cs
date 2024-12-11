using System.Collections.Generic;
using System.Linq;
using BeatSaber.GameSettings;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class GenericSliderSettingTag<T> : BSMLTag
        where T : GenericSliderSetting
    {
        private LayoutElement controllersTransformTemplate;

        public override GameObject CreateObject(Transform parent)
        {
            if (controllersTransformTemplate == null)
            {
                controllersTransformTemplate = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos.First(m => m.viewController is ControllerProfilesSettingsViewController).viewController.transform.Find("Content/MainContent/Sliders/PositionX").GetComponent<LayoutElement>();
            }

            LayoutElement baseSetting = Object.Instantiate(controllersTransformTemplate, parent, false);
            baseSetting.name = "BSMLSliderSetting";

            RectTransform rectTransform = (RectTransform)baseSetting.transform;
            rectTransform.anchoredPosition = Vector3.zero;

            Object.Destroy(rectTransform.Find("SliderLeft").gameObject);
            Object.Destroy(baseSetting.GetComponent<CanvasGroup>());

            GameObject gameObject = baseSetting.gameObject;

            T sliderSetting = gameObject.AddComponent<T>();

            sliderSetting.Slider = baseSetting.GetComponentInChildren<CustomFormatRangeValuesSlider>();
            sliderSetting.Slider.name = "BSMLSlider";
            sliderSetting.Slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            sliderSetting.Slider._enableDragging = true;

            RectTransform sliderTransform = (RectTransform)sliderSetting.Slider.transform;
            sliderTransform.anchorMin = new Vector2(1, 0);
            sliderTransform.anchorMax = new Vector2(1, 1);
            sliderTransform.sizeDelta = new Vector2(52, 0);
            sliderTransform.pivot = new Vector2(1, 0.5f);
            sliderTransform.anchoredPosition = new Vector2(0, 0);

            GameObject titleObject = gameObject.transform.Find("Title").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(titleObject);

            RectTransform titleTransform = (RectTransform)titleObject.transform;
            titleTransform.anchorMin = Vector3.zero;
            titleTransform.anchorMax = Vector3.zero;
            titleTransform.offsetMin = Vector3.zero;
            titleTransform.offsetMax = new Vector2(-52, 0);

            TextMeshProUGUI titleTextMesh = titleObject.GetComponent<TextMeshProUGUI>();
            titleTextMesh.text = "Default Text";
            titleTextMesh.rectTransform.anchorMax = Vector2.one;
            titleTextMesh.alignment = TextAlignmentOptions.CaplineLeft;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().Components;
            externalComponents.Add(titleTextMesh);
            externalComponents.Add(localizedText);

            baseSetting.preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
