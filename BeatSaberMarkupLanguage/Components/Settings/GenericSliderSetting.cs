using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericSetting
    {
        public RangeValuesTextSlider slider;
        protected TextMeshProUGUI text;
    }
}
