using TMPro;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class IncDecSetting : GenericInteractableSetting
    {
        public TextMeshProUGUI text;
        public Button decButton;
        public Button incButton;

        private bool _interactable = true;
        private bool decEnabled;
        private bool incEnabled;

        public override bool interactable
        {
            get => _interactable;
            set
            {
                if (_interactable == value)
                {
                    return;
                }

                _interactable = value;
                EnableDec = decEnabled;
                EnableInc = incEnabled;
            }
        }

        public bool EnableDec
        {
            set
            {
                decEnabled = value;
                decButton.interactable = value && interactable;
            }
        }

        public bool EnableInc
        {
            set
            {
                incEnabled = value;
                incButton.interactable = value && interactable;
            }
        }

        public string Text
        {
            set => text.text = value;
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
