using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        [SerializeField]
        private RangeValuesTextSlider slider;

        [SerializeField]
        private bool showButtons = false;

        [SerializeField]
        private TextMeshProUGUI text;

        private Button incButton;
        private Button decButton;

        public RangeValuesTextSlider Slider
        {
            get => slider;
            set => slider = value;
        }

        public bool ShowButtons
        {
            get => showButtons;
            set => showButtons = value;
        }

        public override bool Interactable
        {
            get => slider != null && ((TextSlider)slider).interactable;
            set
            {
                if (slider != null)
                {
                    ((TextSlider)slider).interactable = value;
                    if (showButtons)
                    {
                        incButton.interactable = value;
                        decButton.interactable = value;
                    }
                }
            }
        }

        protected TextMeshProUGUI Text
        {
            get => text;
            set => text = value;
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

        protected void Awake()
        {
            if (Slider != null)
            {
                Slider.Refresh();
            }
        }
    }
}
