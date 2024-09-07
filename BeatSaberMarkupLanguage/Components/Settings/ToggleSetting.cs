using TMPro;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ToggleSetting : GenericInteractableSetting
    {
        public Toggle Toggle;
        public TextMeshProUGUI TextMesh;

        private bool currentValue;

        public bool Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                Toggle.isOn = value;
            }
        }

        public string Text
        {
            get => TextMesh.text;
            set => TextMesh.text = value;
        }

        public override bool Interactable
        {
            get => Toggle.interactable;
            set => Toggle.interactable = value;
        }

        public override void Setup()
        {
            ReceiveValue();
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

        private void OnEnable()
        {
            Toggle.onValueChanged.AddListener(OnValueChanged);
            Toggle.isOn = currentValue;
        }

        private void OnDisable()
        {
            Toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            Value = value;

            OnChange?.Invoke(Value);

            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }
    }
}
