using Polyglot;
using System;

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

        private void EitherPressed()
        {
            onChange?.Invoke(Value);
            if (updateOnChange)
                ApplyValue();
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                Value = (bool)associatedValue.GetValue();
        }

        private void UpdateState()
        {
            EnableDec = currentValue;
            EnableInc = !currentValue;
            if (formatter != null)
                Text = formatter.Invoke(currentValue) as string;
            else
                Text = currentValue ? Localization.Get("SETTINGS_ON") : Localization.Get("SETTINGS_OFF");
        }
    }
}
