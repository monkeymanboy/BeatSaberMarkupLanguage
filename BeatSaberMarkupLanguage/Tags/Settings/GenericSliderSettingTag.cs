using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using Polyglot;
using System;
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
            BoolSettingsController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<BoolSettingsController>().First(x => (x.name == "Fullscreen")), parent, false);
            baseSetting.name = "BSMLSliderSetting";

            GameObject gameObject = baseSetting.gameObject;

            T sliderSetting = gameObject.AddComponent<T>();
            Transform valuePick = gameObject.transform.Find("ValuePicker");
            sliderSetting.slider = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.TimeSlider>().First(s => s.name != "BSMLSlider"), valuePick, false);
            sliderSetting.slider.name = "BSMLSlider";
            sliderSetting.slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (sliderSetting.slider.transform as RectTransform).anchorMin = new Vector2(-0.2f, 0.4f);
            (sliderSetting.slider.transform as RectTransform).anchorMax = new Vector2(1, 1.2f);
            (sliderSetting.slider.transform as RectTransform).sizeDelta = new Vector2(0, 0);
            // This must be attached to RangeValuesTextSlider's GameObject to receive the Unity EventSystem events.
            sliderSetting.dragHelper = sliderSetting.slider.gameObject.AddComponent<DragHelper>();
            sliderSetting.gameObject.AddComponent<ExternalComponents>().components.Add(sliderSetting.dragHelper);
            MonoBehaviour.Destroy(baseSetting);
            GameObject.Destroy(valuePick.GetComponentsInChildren<TextMeshProUGUI>().First().transform.parent.gameObject);
            GameObject.Destroy(valuePick.GetComponentsInChildren<Button>().First().gameObject);
            GameObject.Destroy(valuePick.GetComponentsInChildren<Button>().Last().gameObject);

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Default Text";
            gameObject.AddComponent<ExternalComponents>().components.Add(text);
            MonoBehaviour.Destroy(text.GetComponent<LocalizedTextMeshProUGUI>());


            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
