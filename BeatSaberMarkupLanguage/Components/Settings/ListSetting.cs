using System.Collections;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSetting : IncDecSetting
    {
        public IList values;

        private int index;

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

        private void ValidateRange()
        {
            if (index >= values.Count)
            {
                index = values.Count - 1;
            }

            if (index < 0)
            {
                index = 0;
            }
        }

        private void UpdateState()
        {
            EnableDec = index > 0;
            EnableInc = index < values.Count - 1;
            Text = formatter == null ? Value.ToString() : (formatter.Invoke(Value) as string);
        }
    }
}
