using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class IncDecSetting : GenericInteractableSetting
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;

        [SerializeField]
        private Button decButton;

        [SerializeField]
        private Button incButton;

        private bool interactable = true;
        private bool decEnabled;
        private bool incEnabled;

        public TextMeshProUGUI TextMesh
        {
            get => textMesh;
            set => textMesh = value;
        }

        public Button DecButton
        {
            get => decButton;
            set => decButton = value;
        }

        public Button IncButton
        {
            get => incButton;
            set => incButton = value;
        }

        public override bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable == value)
                {
                    return;
                }

                interactable = value;
                EnableDec = decEnabled;
                EnableInc = incEnabled;
            }
        }

        public bool EnableDec
        {
            set
            {
                decEnabled = value;
                decButton.interactable = value && Interactable;
            }
        }

        public bool EnableInc
        {
            set
            {
                incEnabled = value;
                incButton.interactable = value && Interactable;
            }
        }

        public string Text
        {
            set => textMesh.text = value;
        }

        public abstract void IncButtonPressed();

        public abstract void DecButtonPressed();

        protected virtual void OnEnable()
        {
            incButton.onClick.AddListener(IncButtonPressed);
            decButton.onClick.AddListener(DecButtonPressed);
        }

        protected void OnDisable()
        {
            incButton.onClick.RemoveListener(IncButtonPressed);
            decButton.onClick.RemoveListener(DecButtonPressed);
        }
    }
}
