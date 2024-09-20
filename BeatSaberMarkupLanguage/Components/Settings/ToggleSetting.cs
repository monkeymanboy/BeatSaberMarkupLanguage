using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ToggleSetting : GenericInteractableSetting
    {
        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        private TextMeshProUGUI textMesh;

        private bool currentValue;

        public Toggle Toggle
        {
            get => toggle;
            set => toggle = value;
        }

        public TextMeshProUGUI TextMesh
        {
            get => textMesh;
            set => textMesh = value;
        }

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
            get => textMesh.text;
            set => textMesh.text = value;
        }

        public override bool Interactable
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
            AssociatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Value = (bool)AssociatedValue.GetValue();
            }
        }

        protected void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
            toggle.isOn = currentValue;
        }

        protected void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
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
