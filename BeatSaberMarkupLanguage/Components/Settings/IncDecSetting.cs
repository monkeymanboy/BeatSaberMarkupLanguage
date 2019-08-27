using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class IncDecSetting : MonoBehaviour
    {
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange = false;

        public TextMeshProUGUI label;
        public TextMeshProUGUI text;
        public Button decButton;
        public Button incButton;

        public bool EnableDec
        {
            set
            {
                decButton.interactable = value;
            }
        }

        public bool EnableInc
        {
            set
            {
                incButton.interactable = value;
            }
        }

        public string Text
        {
            set
            {
                text.text = value;
            }
        }

        public string LabelText
        {
            set
            {
                label.text = value;
            }
        }

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

        public abstract void IncButtonPressed();

        public abstract void DecButtonPressed();
    }
}
