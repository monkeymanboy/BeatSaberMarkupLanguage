using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeatSaberMarkupLanguage.Parser;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class CheckboxSetting : MonoBehaviour
    {
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange = false;
        
        public Toggle checkbox;

        public bool EnableCheckbox
        {
            set => checkbox.interactable = value;
        }

        public bool CheckboxValue
        {
            set => checkbox.isOn = value;
        }

        protected virtual void OnEnable()
        {
            checkbox.onValueChanged.AddListener(CheckboxToggled);
        }

        protected void OnDisable()
        {
            checkbox.onValueChanged.RemoveListener(CheckboxToggled);
        }

        public void Setup()
        {
            ReceiveValue();
        }

        public void CheckboxToggled(bool value)
        {
            onChange?.Invoke(checkbox.isOn);
            if (updateOnChange) ApplyValue();
        }

        public void ApplyValue()
        {  //Mainly I do this so that it doesnt trigger after initially grabbing the value.
            if (checkbox.isOn != (bool)associatedValue?.GetValue())
                associatedValue.SetValue(checkbox.isOn);
        }

        public void ReceiveValue()
        {
            CheckboxValue = (bool)associatedValue?.GetValue();
        }
    }
}
