using System;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class IncrementSetting : IncDecSetting
    {
        public bool IsInt;
        public float MinValue = float.MinValue;
        public float MaxValue = float.MaxValue;
        public float Increments = 1;

        private float currentValue;

        public float Value
        {
            get
            {
                ValidateRange();
                return currentValue;
            }

            set
            {
                if (IsInt)
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

            if (IsInt)
            {
                MinValue = ConvertToInt(MinValue);
                MaxValue = ConvertToInt(MaxValue);
                Increments = ConvertToInt(Increments);
            }
        }

        public override void DecButtonPressed()
        {
            currentValue -= Increments;
            EitherPressed();
        }

        public override void IncButtonPressed()
        {
            currentValue += Increments;
            EitherPressed();
        }

        public override void ApplyValue()
        {
            if (AssociatedValue != null)
            {
                if (IsInt)
                {
                    AssociatedValue.SetValue(Convert.ToInt32(Value));
                }
                else
                {
                    AssociatedValue.SetValue(Value);
                }
            }
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Value = IsInt ? Convert.ToInt32(AssociatedValue.GetValue()) : Convert.ToSingle(AssociatedValue.GetValue());
            }
        }

        protected string TextForValue(float value)
        {
            if (IsInt)
            {
                return Formatter == null ? ConvertToInt(value).ToString() : (Formatter.Invoke(ConvertToInt(value)) as string);
            }
            else
            {
                return Formatter == null ? value.ToString("N2") : (Formatter.Invoke(value) as string);
            }
        }

        private void EitherPressed()
        {
            UpdateState();
            if (IsInt)
            {
                OnChange?.Invoke(Convert.ToInt32(Value));
            }
            else
            {
                OnChange?.Invoke(Value);
            }

            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        private void ValidateRange()
        {
            if (currentValue < MinValue)
            {
                currentValue = MinValue;
            }
            else if (currentValue > MaxValue)
            {
                currentValue = MaxValue;
            }
        }

        private void UpdateState()
        {
            ValidateRange();

            EnableDec = currentValue > MinValue;
            EnableInc = currentValue < MaxValue;
            Text = TextForValue(currentValue);
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
