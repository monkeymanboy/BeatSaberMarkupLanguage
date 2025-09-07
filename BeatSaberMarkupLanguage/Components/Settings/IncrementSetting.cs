using System;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class IncrementSetting : IncDecSetting
    {
        [SerializeField]
        private bool isInt;

        [SerializeField]
        private float minValue = float.MinValue;

        [SerializeField]
        private float maxValue = float.MaxValue;

        [SerializeField]
        private float increments = 1;

        private float currentValue;

        public bool IsInt
        {
            get => isInt;
            set => isInt = value;
        }

        public float MinValue
        {
            get => minValue;
            set => minValue = value;
        }

        public float MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        public float Increments
        {
            get => increments;
            set => increments = value;
        }

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

        public override void ApplyValue()
        {
            if (AssociatedValue != null)
            {
                if (isInt)
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
                Value = isInt ? Convert.ToInt32(AssociatedValue.GetValue()) : Convert.ToSingle(AssociatedValue.GetValue());
            }
            else
            {
                UpdateState();
            }
        }

        protected string TextForValue(float value)
        {
            if (isInt)
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
            if (isInt)
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
