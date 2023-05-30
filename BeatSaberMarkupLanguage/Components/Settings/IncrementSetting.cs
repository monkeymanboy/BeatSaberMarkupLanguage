using System;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class IncrementSetting : IncDecSetting
    {
        private float currentValue;

        public bool isInt;
        public float minValue = float.MinValue;
        public float maxValue = float.MaxValue;
        public float increments = 1;

        public float Value
        {
            get
            {
                ValidateRange();
                return currentValue;
            }

            set
            {
                if (isInt)
                {
                    currentValue = Convert.ToInt32(value);
                }
                else
                {
                    currentValue = value;
                }

                UpdateState();
            }
        }

        public override void Setup()
        {
            ReceiveValue();

            if (isInt)
            {
                minValue = ConvertToInt(minValue);
                maxValue = ConvertToInt(maxValue);
                increments = ConvertToInt(increments);
            }
        }

        public override void DecButtonPressed()
        {
            currentValue -= increments;
            EitherPressed();
        }

        public override void IncButtonPressed()
        {
            currentValue += increments;
            EitherPressed();
        }

        private void EitherPressed()
        {
            UpdateState();
            if (isInt)
            {
                onChange?.Invoke(Convert.ToInt32(Value));
            }
            else
            {
                onChange?.Invoke(Value);
            }

            if (updateOnChange)
            {
                ApplyValue();
            }
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
            {
                if (isInt)
                {
                    associatedValue.SetValue(Convert.ToInt32(Value));
                }
                else
                {
                    associatedValue.SetValue(Value);
                }
            }
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = isInt ? Convert.ToInt32(associatedValue.GetValue()) : Convert.ToSingle(associatedValue.GetValue());
            }
        }

        private void ValidateRange()
        {
            if (currentValue < minValue)
            {
                currentValue = minValue;
            }
            else if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
        }

        private void UpdateState()
        {
            ValidateRange();

            EnableDec = currentValue > minValue;
            EnableInc = currentValue < maxValue;
            Text = TextForValue(currentValue);
        }

        protected string TextForValue(float value)
        {
            if (isInt)
            {
                return formatter == null ? ConvertToInt(value).ToString() : (formatter.Invoke(ConvertToInt(value)) as string);
            }
            else
            {
                return formatter == null ? value.ToString("N2") : (formatter.Invoke(value) as string);
            }
        }

        private int ConvertToInt(float value)
        {
            int result;

            if (value < int.MinValue)
            {
                result = int.MinValue;
            }
            else if (value > int.MaxValue)
            {
                result = int.MaxValue;
            }
            else
            {
                result = Convert.ToInt32(value);
            }

            return result;
        }
    }
}
