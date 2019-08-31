using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            set
            {
                label.text = value;
            }
        }
    }
}
