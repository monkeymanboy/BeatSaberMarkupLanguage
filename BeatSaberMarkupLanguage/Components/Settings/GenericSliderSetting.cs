using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        public RangeValuesTextSlider slider;
        protected TextMeshProUGUI text;
        public override bool interactable 
        { 
            get => slider?.interactable ?? false;
            set
            {
                if(slider != null)
                {
                    slider.interactable = value;
                }
            }
        }
    }
}
