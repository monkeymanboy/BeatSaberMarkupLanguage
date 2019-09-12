using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSetting : IncDecSetting
    {
        private int index;

        public BSMLAction formatter;
        public List<object> values;

        public object Value
        {
            get
            {
                ValidateRange();
                return values[index];
            }
            set
            {
                index = values.IndexOf(value);
                if (index < 0)
                    index = 0;

                UpdateState();
            }
        }

        public void Setup()
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

        private void EitherPressed()
        {
            UpdateState();
            onChange?.Invoke(Value);
            if (updateOnChange)
                ApplyValue();
        }

        public void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Value);
        }

        public void ReceiveValue()
        {
            if (associatedValue != null)
                Value = associatedValue.GetValue();
        }

        private void ValidateRange()
        {
            if (index >= values.Count)
                index = values.Count - 1;

            if (index < 0)
                index = 0;
        }

        private void UpdateState()
        {
            EnableDec = index > 0;
            EnableInc = index < values.Count - 1;
            Text = formatter == null ? Value.ToString() : (formatter.Invoke(Value) as string);
        }
    }
}
