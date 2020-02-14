using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class StringSetting : GenericSetting
    {   
        public TextMeshProUGUI text;
        public Button editButton;

        public RectTransform boundingBox;
        public ModalKeyboard modalKeyboard;

        private string currentValue;

        public string Text
        {
            get => currentValue;
            set
            {
                currentValue = value;
                text.text = formatter == null ? value : formatter.Invoke(value) as string;
            }
        }

        void Update()//TODO: Remove need for this to be called in Update
        {
            boundingBox.sizeDelta = new Vector2(text.textBounds.size.x + 7, 0);
        }

        public override void Setup()
        {
            modalKeyboard.clearOnOpen = false;
            ReceiveValue();
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
                ApplyValue();
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Text);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                Text = (string)associatedValue.GetValue();
        }
    }
}
