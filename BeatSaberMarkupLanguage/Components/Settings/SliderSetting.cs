using System;
using BeatSaberMarkupLanguage.Harmony_Patches;
using HMUI;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class SliderSetting : GenericSliderSetting
    {
        public bool IsInt = false;
        public float Increments;

        private float lastValue = float.NegativeInfinity;

        public float Value
        {
            get => Slider.value;
            set => Slider.value = value;
        }

        public override void Setup()
        {
            base.Setup();

            ApplyCustomSliderTexts.Remappers.Add(Slider, this);

            text = Slider.GetComponentInChildren<TextMeshProUGUI>();
            Slider.numberOfSteps = (int)Math.Round((Slider.maxValue - Slider.minValue) / Increments) + 1;
            Slider.valueDidChangeEvent += OnValueChanged;

            // TextSlider.UpdateVisuals doesn't work properly when disabled
            if (Slider.gameObject.activeInHierarchy)
            {
                ReceiveValue();
            }
        }

        public override void ApplyValue()
        {
            if (AssociatedValue != null)
            {
                if (IsInt)
                {
                    AssociatedValue.SetValue((int)Math.Round(Slider.value));
                }
                else
                {
                    AssociatedValue.SetValue(Slider.value);
                }
            }
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Slider.value = IsInt ? (int)AssociatedValue.GetValue() : (float)AssociatedValue.GetValue();
            }
        }

        internal string TextForValue(float value)
        {
            if (IsInt)
            {
                return Formatter == null ? ((int)Math.Round(value)).ToString() : (Formatter.Invoke((int)Math.Round(value)) as string);
            }
            else
            {
                return Formatter == null ? value.ToString("N2") : (Formatter.Invoke(value) as string);
            }
        }

        protected void Awake()
        {
            ReceiveValue();
        }

        private void OnValueChanged(TextSlider textSlider, float val)
        {
            if (IsInt)
            {
                val = (int)Math.Round(val);
            }

            if (lastValue == val)
            {
                return;
            }

            lastValue = val;

            if (IsInt)
            {
                OnChange?.Invoke((int)val);
            }
            else
            {
                OnChange?.Invoke(val);
            }

            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }
    }
}
