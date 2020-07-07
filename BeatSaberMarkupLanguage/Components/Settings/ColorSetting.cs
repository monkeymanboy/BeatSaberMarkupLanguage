using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ColorSetting : GenericInteractableSetting
    {
        public Button editButton;
        public ModalColorPicker modalColorPicker;
        public Image colorImage;

        private Color _currentColor = Color.white;
        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                if (colorImage != null)
                    colorImage.color = _currentColor;
            }
        }

        public override bool interactable
        {
            get => editButton?.interactable ?? false;
            set
            {
                if (editButton != null)
                    editButton.interactable = value;
            }
        }

        public override void Setup()
        {
            modalColorPicker.onChange = onChange;
            modalColorPicker.doneEvent += DonePressed;
            modalColorPicker.cancelEvent += CancelPressed;
            ReceiveValue();
        }

        protected virtual void OnEnable()
        {
            editButton.onClick.AddListener(EditButtonPressed);
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
        }

        public void EditButtonPressed()
        {
            modalColorPicker.CurrentColor = CurrentColor;
            modalColorPicker.modalView.Show(true, true);
        }

        public void DonePressed(Color color)
        {
            CurrentColor = color;
            if (updateOnChange)
                ApplyValue();
        }

        public void CancelPressed()
        {
            onChange?.Invoke(CurrentColor);
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(CurrentColor);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                CurrentColor = (Color)associatedValue.GetValue();
        }
    }
}
