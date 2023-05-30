using TMPro;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ToggleSetting : GenericInteractableSetting
    {
        public Toggle toggle;
        public TextMeshProUGUI text;

        private bool currentValue;

        public bool Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                toggle.isOn = value;
            }
        }

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public override bool interactable
        {
            get => toggle.interactable;
            set => toggle.interactable = value;
        }

        public override void Setup()
        {
            ReceiveValue();
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = (bool)associatedValue.GetValue();
            }
        }

        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
            toggle.isOn = currentValue;
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            Value = value;

            onChange?.Invoke(Value);

            if (updateOnChange)
            {
                ApplyValue();
            }
        }
    }
}
