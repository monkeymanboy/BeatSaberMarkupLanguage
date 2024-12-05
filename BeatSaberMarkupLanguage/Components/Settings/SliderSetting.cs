using System;
using BeatSaberMarkupLanguage.Harmony_Patches;
using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class SliderSetting : GenericSliderSetting
    {
        [SerializeField]
        private bool isInt = false;

        [SerializeField]
        private float increments;

        private float lastValue = float.NegativeInfinity;

        public bool IsInt
        {
            get => isInt;
            set => isInt = value;
        }

        public float Increments
        {
            get => increments;
            set => increments = value;
        }

        public float Value
        {
            get => Slider.value;
            set => Slider.value = value;
        }

        public override void Setup()
        {
            base.Setup();

            ApplyCustomSliderTexts.Remappers.Add(Slider, this);

            Text = Slider.GetComponentInChildren<TextMeshProUGUI>();
            Slider.numberOfSteps = (int)Math.Round((Slider.maxValue - Slider.minValue) / increments) + 1;
            Slider.valueDidChangeEvent += OnValueChanged;

            ReceiveValue();
        }

        public override void ApplyValue()
        {
            if (AssociatedValue != null)
            {
                if (isInt)
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
                Slider.value = isInt ? (int)AssociatedValue.GetValue() : (float)AssociatedValue.GetValue();
            }
        }

        internal string TextForValue(float value)
        {
            if (isInt)
            {
                return Formatter == null ? ((int)Math.Round(value)).ToString() : (Formatter.Invoke((int)Math.Round(value)) as string);
            }
            else
            {
                return Formatter == null ? value.ToString("N2") : (Formatter.Invoke(value) as string);
            }
        }

        private void OnValueChanged(TextSlider textSlider, float val)
        {
            if (isInt)
            {
                val = (int)Math.Round(val);
            }

            if (lastValue == val)
            {
                return;
            }

            lastValue = val;

            if (isInt)
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
