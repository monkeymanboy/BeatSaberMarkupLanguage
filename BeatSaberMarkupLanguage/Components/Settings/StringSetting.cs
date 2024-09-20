using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class StringSetting : GenericInteractableSetting
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;

        [SerializeField]
        private Button editButton;

        [SerializeField]
        private RectTransform boundingBox;

        private string currentValue;

        public TextMeshProUGUI TextMesh
        {
            get => textMesh;
            set => textMesh = value;
        }

        public Button EditButton
        {
            get => editButton;
            set => editButton = value;
        }

        public RectTransform BoundingBox
        {
            get => boundingBox;
            set => boundingBox = value;
        }

        public ModalKeyboard ModalKeyboard { get; set; }

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

        public string Text
        {
            get => currentValue;
            set
            {
                currentValue = value;
                textMesh.text = Formatter == null ? value : Formatter.Invoke(value) as string;
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
            editButton.onClick.AddListener(EditButtonPressed);
            ModalKeyboard.Keyboard.EnterPressed += EnterPressed;
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
            ModalKeyboard.Keyboard.EnterPressed -= EnterPressed;
        }
    }
}
