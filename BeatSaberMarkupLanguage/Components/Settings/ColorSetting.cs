using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ColorSetting : GenericInteractableSetting
    {
        [SerializeField]
        private Button editButton;

        [SerializeField]
        private Image colorImage;

        private Color currentColor = Color.white;

        public ModalColorPicker ModalColorPicker { get; set; }

        public Button EditButton
        {
            get => editButton;
            set => editButton = value;
        }

        public Image ColorImage
        {
            get => colorImage;
            set => colorImage = value;
        }

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

        public override bool Interactable
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
            editButton.onClick.AddListener(EditButtonPressed);
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
        }
    }
}
