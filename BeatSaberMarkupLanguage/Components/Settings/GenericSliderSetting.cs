using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public RangeValuesTextSlider slider;

        protected TextMeshProUGUI text;

        public string LabelText
        {
            set => label.text = value;
        }
    }
}
