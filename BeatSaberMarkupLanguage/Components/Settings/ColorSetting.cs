using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ColorSetting : GenericInteractableSetting
    {
        public Button editButton;
        public ModalColorPicker modalColorPicker;
        public Image colorImage;

        private Color currentColor = Color.white;

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                if (colorImage != null)
                {
                    colorImage.color = currentColor;
                }
            }
        }

        public override bool interactable
        {
            get => editButton != null && editButton.interactable;
            set
            {
                if (editButton != null)
                {
                    editButton.interactable = value;
                }
            }
        }

        public override void Setup()
        {
            modalColorPicker.onChange = onChange;
            modalColorPicker.doneEvent += DonePressed;
            modalColorPicker.cancelEvent += CancelPressed;
            ReceiveValue();
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
            {
                ApplyValue();
            }
        }

        public void CancelPressed()
        {
            onChange?.Invoke(CurrentColor);
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(CurrentColor);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                CurrentColor = (Color)associatedValue.GetValue();
            }
        }

        protected virtual void OnEnable()
        {
            editButton.onClick.AddListener(EditButtonPressed);
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
        }
    }
}
