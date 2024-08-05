using System;
using System.Collections;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSetting : IncDecSetting
    {
        private int index;

        public IList values { get; set; } = Array.Empty<object>();

        public object Value
        {
            get
            {
                return values.Count > 0 ? values[Mathf.Clamp(index, 0, values.Count - 1)] : null;
            }

            set
            {
                index = values.IndexOf(value);
                if (index < 0)
                {
                    index = 0;
                }

                UpdateState();
            }
        }

        public override void Setup()
        {
            ReceiveValue();
        }

        public override void DecButtonPressed()
        {
            index--;
            EitherPressed();
        }

        public override void IncButtonPressed()
        {
            index++;
            EitherPressed();
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

        private void EitherPressed()
        {
            UpdateState();
            onChange?.Invoke(Value);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }

        private void UpdateState()
        {
            EnableDec = index > 0;
            EnableInc = index < values.Count - 1;
            Text = Value != null ? (formatter == null ? Value?.ToString() : (formatter.Invoke(Value) as string)) : string.Empty;
        }
    }
}
