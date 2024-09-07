using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        public RangeValuesTextSlider Slider;
        public bool ShowButtons = false;

        protected TextMeshProUGUI text;

        private Button incButton;
        private Button decButton;

        public override bool Interactable
        {
            get => Slider != null && Slider.interactable;
            set
            {
                if (Slider != null)
                {
                    Slider.interactable = value;
                    if (ShowButtons)
                    {
                        incButton.interactable = value;
                        decButton.interactable = value;
                    }
                }
            }
        }

        public override void Setup()
        {
            incButton = Slider._incButton;
            decButton = Slider._decButton;

            if (!ShowButtons)
            {
                Slider.image.sprite = Utilities.FindSpriteCached("RoundRect10");
                Destroy(incButton.gameObject);
                Destroy(decButton.gameObject);
                (Slider.transform.Find("BG") as RectTransform).sizeDelta = new Vector2(0, 6);
                (Slider.transform as RectTransform).sizeDelta = new Vector2(38, 0);
                (Slider.transform.Find("SlidingArea") as RectTransform).sizeDelta = new Vector2(-4, -4);
            }
        }
    }
}
