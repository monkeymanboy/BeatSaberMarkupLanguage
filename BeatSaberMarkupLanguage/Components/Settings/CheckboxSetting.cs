using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class CheckboxSetting : GenericSetting
    {
        public Toggle checkbox;

        public bool EnableCheckbox
        {
            set => checkbox.interactable = value;
        }

        public bool CheckboxValue
        {
            set => checkbox.isOn = value;
        }

        protected void OnEnable()
        {
            checkbox.onValueChanged.AddListener(CheckboxToggled);
        }

        protected void OnDisable()
        {
            checkbox.onValueChanged.RemoveListener(CheckboxToggled);
        }

        public override void Setup()
        {
            ReceiveValue();
        }

        public void CheckboxToggled(bool value)
        {
            onChange?.Invoke(checkbox.isOn);
            if (updateOnChange) ApplyValue();
        }

        public override void ApplyValue()
        {  //Mainly I do this so that it doesnt trigger after initially grabbing the value.
            if (associatedValue != null && checkbox.isOn != (bool)associatedValue.GetValue())
                associatedValue.SetValue(checkbox.isOn);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                CheckboxValue = (bool)associatedValue.GetValue();
        }
    }
}
