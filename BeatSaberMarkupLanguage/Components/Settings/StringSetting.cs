using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class StringSetting : GenericInteractableSetting
    {
        public TextMeshProUGUI TextMesh;
        public Button EditButton;

        public RectTransform BoundingBox;
        public ModalKeyboard ModalKeyboard;

        private string currentValue;

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

        public string Text
        {
            get => currentValue;
            set
            {
                currentValue = value;
                TextMesh.text = Formatter == null ? value : Formatter.Invoke(value) as string;
            }
        }

        public override void Setup()
        {
            ModalKeyboard.ClearOnOpen = false;
            ReceiveValue();
        }

        public void EditButtonPressed()
        {
            ModalKeyboard.ModalView.Show(true, true);
            ModalKeyboard.SetText(Text);
        }

        public void EnterPressed(string text)
        {
            Text = text;
            OnChange?.Invoke(Text);
            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        public override void ApplyValue()
        {
            AssociatedValue?.SetValue(Text);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Text = (string)AssociatedValue.GetValue();
            }
        }

        protected virtual void OnEnable()
        {
            EditButton.onClick.AddListener(EditButtonPressed);
            ModalKeyboard.Keyboard.EnterPressed += EnterPressed;
        }

        protected void OnDisable()
        {
            EditButton.onClick.RemoveListener(EditButtonPressed);
            ModalKeyboard.Keyboard.EnterPressed -= EnterPressed;
        }
    }
}
