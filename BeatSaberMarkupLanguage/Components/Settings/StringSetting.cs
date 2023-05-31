using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class StringSetting : GenericInteractableSetting
    {
        public TextMeshProUGUI text;
        public Button editButton;

        public RectTransform boundingBox;
        public ModalKeyboard modalKeyboard;

        private string currentValue;

        public override bool interactable
        {
            get => editButton?.interactable ?? false;
            set
            {
                if (editButton != null)
                {
                    editButton.interactable = value;
                }
            }
        }

        public string Text
        {
            get => currentValue;
            set
            {
                currentValue = value;
                text.text = formatter == null ? value : formatter.Invoke(value) as string;
            }
        }

        public override void Setup()
        {
            modalKeyboard.clearOnOpen = false;
            ReceiveValue();
        }

        public void EditButtonPressed()
        {
            modalKeyboard.modalView.Show(true, true);
            modalKeyboard.SetText(Text);
        }

        public void EnterPressed(string text)
        {
            Text = text;
            onChange?.Invoke(Text);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(Text);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Text = (string)associatedValue.GetValue();
            }
        }

        protected virtual void OnEnable()
        {
            editButton.onClick.AddListener(EditButtonPressed);
            modalKeyboard.keyboard.EnterPressed += EnterPressed;
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
            modalKeyboard.keyboard.EnterPressed -= EnterPressed;
        }
    }
}
