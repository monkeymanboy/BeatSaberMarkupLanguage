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
                    currentValue = Convert.ToInt32(value);
                else
                    currentValue = value;

                UpdateState();
            }
        }

        public void Setup()
        {
            ReceiveValue();

            if (isInt)
            {
                minValue = ConvertToInt(minValue, int.MinValue);
                maxValue = ConvertToInt(maxValue, int.MaxValue);
                increments = ConvertToInt(increments, 1);
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
            onChange?.Invoke(Value);
            if (updateOnChange)
                ApplyValue();
        }

        public void ApplyValue()
        {
            if (associatedValue != null)
            {
                if (isInt)
                    associatedValue.SetValue(Convert.ToInt32(Value));
                else
                    associatedValue.SetValue(Value);
            }
        }

        public void ReceiveValue()
        {
            if (associatedValue != null)
                Value = isInt ? Convert.ToInt32(associatedValue.GetValue()) : Convert.ToSingle(associatedValue.GetValue());
        }

        private void ValidateRange()
        {
            if (currentValue < minValue)
                currentValue = minValue;
            else if (currentValue > maxValue)
                currentValue = maxValue;
        }

        private void UpdateState()
        {
            ValidateRange();

            EnableDec = currentValue > minValue;
            EnableInc = currentValue < maxValue;
            Text = currentValue.ToString();
        }

        private int ConvertToInt(object value, int defaultValue = default)
        {
            int result;

            try
            {
                result = Convert.ToInt32(value);
            }
            catch (OverflowException)
            {
                // Value is not within the limits of an Integer
                result = defaultValue;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
