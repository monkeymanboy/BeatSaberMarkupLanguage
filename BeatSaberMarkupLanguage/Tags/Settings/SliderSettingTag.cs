using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SliderSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "slider-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            BoolSettingsController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<BoolSettingsController>().First(x => (x.name == "Fullscreen")), parent, false);
            baseSetting.name = "BSMLSliderSetting";
            GameObject gameObject = baseSetting.gameObject;
            SliderSetting sliderSetting = gameObject.AddComponent<SliderSetting>();

            sliderSetting.slider = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.TimeSlider>().First(s => s.name != "BSMLSlider"), gameObject.transform.Find("Value"), false);
            sliderSetting.slider.name = "BSMLSlider";
            sliderSetting.slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (sliderSetting.slider.transform as RectTransform).sizeDelta = new Vector2(44, 7);
            (sliderSetting.slider.transform as RectTransform).anchorMin = new Vector2(0, 0.5f);

            MonoBehaviour.Destroy(baseSetting);
            GameObject.Destroy(gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First().gameObject);
            GameObject.Destroy(gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First().gameObject);
            GameObject.Destroy(gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last().gameObject);
            sliderSetting.label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            MonoBehaviour.Destroy(sliderSetting.label.GetComponent<LocalizedTextMeshProUGUI>());
            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            sliderSetting.LabelText = "Default Text";
            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
