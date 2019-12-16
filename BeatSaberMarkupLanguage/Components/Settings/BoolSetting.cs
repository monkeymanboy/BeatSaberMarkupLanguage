using Polyglot;

namespace BeatSaberMarkupLanguage.Components.Settings
{
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
            if (associatedValue != null)
                associatedValue.SetValue(Value);
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
            Text = currentValue ? Localization.Get("SETTINGS_ON") : Localization.Get("SETTINGS_OFF");
        }
    }
}
