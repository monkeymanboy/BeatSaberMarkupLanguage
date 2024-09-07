using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ColorSetting : GenericInteractableSetting
    {
        public Button EditButton;
        public ModalColorPicker ModalColorPicker;
        public Image ColorImage;

        private Color currentColor = Color.white;

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                if (ColorImage != null)
                {
                    ColorImage.color = currentColor;
                }
            }
        }

        public override bool Interactable
        {
            get => EditButton != null && EditButton.interactable;
            set
            {
                if (EditButton != null)
                {
                    EditButton.interactable = value;
                }
            }
        }

        public override void Setup()
        {
            ModalColorPicker.OnChange = OnChange;
            ModalColorPicker.DoneEvent += DonePressed;
            ModalColorPicker.CancelEvent += CancelPressed;
            ReceiveValue();
        }

        public void EditButtonPressed()
        {
            ModalColorPicker.CurrentColor = CurrentColor;
            ModalColorPicker.ModalView.Show(true, true);
        }

        public void DonePressed(Color color)
        {
            CurrentColor = color;
            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        public void CancelPressed()
        {
            OnChange?.Invoke(CurrentColor);
        }

        public override void ApplyValue()
        {
            AssociatedValue?.SetValue(CurrentColor);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                CurrentColor = (Color)AssociatedValue.GetValue();
            }
        }

        protected virtual void OnEnable()
        {
            EditButton.onClick.AddListener(EditButtonPressed);
        }

        protected void OnDisable()
        {
            EditButton.onClick.RemoveListener(EditButtonPressed);
        }
    }
}
