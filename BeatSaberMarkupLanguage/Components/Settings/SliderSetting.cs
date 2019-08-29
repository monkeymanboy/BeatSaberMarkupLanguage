using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    class SliderSetting : MonoBehaviour
    {
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange = false;
        public bool isInt = false;
        public float increments;

        public TextMeshProUGUI label;
        public RangeValuesTextSlider slider;

        private TextMeshProUGUI text;

        public string LabelText
        {
            set
            {
                label.text = value;
            }
        }

        public void Setup()
        {
            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = (int)((slider.maxValue - slider.minValue) / increments) + 1;
            ReceiveValue();
            slider.valueDidChangeEvent += OnChange;
            StartCoroutine(SetInitialText());
        }
        IEnumerator SetInitialText()//I don't really like this but for some reason I can't get the inital starting text any other quick way and this works perfectly fine
        {
            yield return new WaitForFixedUpdate();
            text.text = TextForValue(slider.value);
        }

        private void OnChange(TextSlider _, float val)
        {
            text.text = TextForValue(slider.value);
            onChange?.Invoke(isInt ? (int)slider.value : slider.value);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }
        public void ApplyValue()
        {
            if (associatedValue != null)
                if (isInt)
                    associatedValue.SetValue((int)slider.value);
                else
                    associatedValue.SetValue(slider.value);
        }
        public void ReceiveValue()
        {
            if (associatedValue != null)
                slider.value = isInt ? (int)associatedValue.GetValue() : (float)associatedValue.GetValue();
            text.text = TextForValue(slider.value);
        }

        protected string TextForValue(float value)
        {
            if (isInt)
                return Math.Floor(value).ToString("N0");
            return value.ToString("N1");
        }

    }
}
