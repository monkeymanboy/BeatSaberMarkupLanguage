using System;
using System.Collections;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSetting : IncDecSetting
    {
        private int index;

        public IList Values { get; set; } = Array.Empty<object>();

        public object Value
        {
            get
            {
                return Values.Count > 0 ? Values[Mathf.Clamp(index, 0, Values.Count - 1)] : null;
            }

            set
            {
                index = Values.IndexOf(value);
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
            AssociatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Value = AssociatedValue.GetValue();
            }
        }

        private void EitherPressed()
        {
            UpdateState();
            OnChange?.Invoke(Value);
            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        private void UpdateState()
        {
            EnableDec = index > 0;
            EnableInc = index < Values.Count - 1;
            Text = Value != null ? (Formatter == null ? Value?.ToString() : (Formatter.Invoke(Value) as string)) : string.Empty;
        }
    }
}
