using BeatSaberMarkupLanguage.Parser;
using IPA.Utilities;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class BoolSetting : IncDecSetting
    {
        private bool currentValue;
        public bool Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                UpdateState();
            }
        }
        public void Setup()
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
            {
                ApplyValue();
            }
        }
        public void ApplyValue()
        {
            if(associatedValue != null)
                associatedValue.SetValue(Value);
        }
        public void ReceiveValue()
        {
            if(associatedValue != null)
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
