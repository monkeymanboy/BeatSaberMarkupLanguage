using TMPro;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class IncDecSetting : GenericInteractableSetting
    {
        public TextMeshProUGUI TextMesh;
        public Button DecButton;
        public Button IncButton;

        private bool interactable = true;
        private bool decEnabled;
        private bool incEnabled;

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
                DecButton.interactable = value && Interactable;
            }
        }

        public bool EnableInc
        {
            set
            {
                incEnabled = value;
                IncButton.interactable = value && Interactable;
            }
        }

        public string Text
        {
            set => TextMesh.text = value;
        }

        public abstract void IncButtonPressed();

        public abstract void DecButtonPressed();

        protected virtual void OnEnable()
        {
            IncButton.onClick.AddListener(IncButtonPressed);
            DecButton.onClick.AddListener(DecButtonPressed);
        }

        protected void OnDisable()
        {
            IncButton.onClick.RemoveListener(IncButtonPressed);
            DecButton.onClick.RemoveListener(DecButtonPressed);
        }
    }
}
