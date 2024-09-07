using System;
using BGLib.Polyglot;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    [Obsolete("This has been replaced with ToggleSetting")]
    public class BoolSetting : IncDecSetting
    {
        private bool currentValue;

        public bool Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                UpdateState();
            }
        }

        public override void Setup()
        {
            ReceiveValue();
        }

        public override void DecButtonPressed()
        {
            Value = false;
            EitherPressed();
        }

        public override void IncButtonPressed()
        {
            Value = true;
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
                Value = (bool)AssociatedValue.GetValue();
            }
        }

        private void EitherPressed()
        {
            OnChange?.Invoke(Value);
            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        private void UpdateState()
        {
            EnableDec = currentValue;
            EnableInc = !currentValue;
            if (Formatter != null)
            {
                Text = Formatter.Invoke(currentValue) as string;
            }
            else
            {
                Text = currentValue ? Localization.Get("SETTINGS_ON") : Localization.Get("SETTINGS_OFF");
            }
        }
    }
}
