using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        public RangeValuesTextSlider slider;
        public bool showButtons = false;

        protected TextMeshProUGUI text;

        private Button incButton;
        private Button decButton;

        public override bool interactable
        {
            get => slider?.interactable ?? false;
            set
            {
                if (slider != null)
                {
                    slider.interactable = value;
                    if (showButtons)
                    {
                        incButton.interactable = value;
                        decButton.interactable = value;
                    }
                }
            }
        }

        public override void Setup()
        {
            incButton = slider._incButton;
            decButton = slider._decButton;

            if (!showButtons)
            {
                slider.image.sprite = Utilities.FindSpriteCached("RoundRect10");
                Destroy(incButton.gameObject);
                Destroy(decButton.gameObject);
                (slider.transform.Find("BG") as RectTransform).sizeDelta = new Vector2(0, 6);
                (slider.transform as RectTransform).sizeDelta = new Vector2(38, 0);
                (slider.transform.Find("SlidingArea") as RectTransform).sizeDelta = new Vector2(-4, -4);
            }
        }
    }
}
