using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        public RangeValuesTextSlider slider;
        protected TextMeshProUGUI text;
        public bool showButtons = false;
        private static Sprite roundRect10; 

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

        public override void Setup()
        {
            if (!showButtons)
            {
                if (roundRect10 == null)
                    roundRect10 = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == "RoundRect10");

                slider.image.sprite = roundRect10;
                GameObject.Destroy(slider.GetField<Button, RangeValuesTextSlider>("_incButton").gameObject);
                GameObject.Destroy(slider.GetField<Button, RangeValuesTextSlider>("_decButton").gameObject);
                slider.transform.localPosition = new Vector3(slider.transform.localPosition.x + 7, 0, 0);
            }
        }
    }
}
