using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSliderSetting : GenericSliderSetting
    {
        public List<object> values;

        public object Value
        {
            get => values[(int)Math.Round(slider.value)];
            set
            {
                slider.value = values.IndexOf(value) * 1f;
                text.text = TextForValue(Value);
            }
        }

        public override void Setup()
        {
            base.Setup();
            slider.minValue = 0;
            slider.maxValue = values.Count() - 1;
            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = values.Count;
            slider.valueDidChangeEvent += OnChange;

            // TextSlider.UpdateVisuals doesn't work properly when disabled
            if (slider.gameObject.activeInHierarchy)
            {
                ReceiveValue();
            }
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = associatedValue.GetValue();
            }
        }

        protected string TextForValue(object value)
        {
            return formatter == null ? value.ToString() : (formatter.Invoke(value) as string);
        }

        private void Awake()
        {
            ReceiveValue();
        }

        private void OnChange(TextSlider textSlider, float val)
        {
            text.text = TextForValue(Value);
            onChange?.Invoke(Value);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }
    }
}
