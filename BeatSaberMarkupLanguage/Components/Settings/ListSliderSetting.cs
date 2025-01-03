﻿using System;
using System.Collections;
using HMUI;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSliderSetting : GenericSliderSetting
    {
        public IList Values { get; set; } = Array.Empty<object>();

        public object Value
        {
            get => Values[(int)Math.Round(Slider.value)];
            set
            {
                Slider.value = Values.IndexOf(value) * 1f;
                Text.text = TextForValue(Value);
            }
        }

        public override void Setup()
        {
            base.Setup();

            Slider.minValue = 0;
            Slider.maxValue = Values.Count - 1;
            Text = Slider.GetComponentInChildren<TextMeshProUGUI>();
            Slider.numberOfSteps = Values.Count;
            Slider.valueDidChangeEvent += OnValueChanged;

            ReceiveValue();
        }

        public override void ApplyValue()
        {
            AssociatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Value = AssociatedValue.GetValue();
            }
        }

        protected string TextForValue(object value)
        {
            return Formatter == null ? value.ToString() : (Formatter.Invoke(value) as string);
        }

        private void OnValueChanged(TextSlider textSlider, float val)
        {
            Text.text = TextForValue(Value);
            OnChange?.Invoke(Value);
            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }
    }
}
