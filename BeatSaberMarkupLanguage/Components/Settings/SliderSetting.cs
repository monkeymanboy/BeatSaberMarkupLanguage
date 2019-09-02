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
    public class SliderSetting : GenericSliderSetting
    {
        public BSMLAction formatter;
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange = false;
        public bool isInt = false;
        public float increments;

        public void Setup()
        {
            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = (int)((slider.maxValue - slider.minValue) / increments) + 1;
            ReceiveValue();
            slider.valueDidChangeEvent += OnChange;
            StartCoroutine(SetInitialText());
        }
        private void OnEnable()
        {
            StartCoroutine(SetInitialText());
        }
        IEnumerator SetInitialText()//I don't really like this but for some reason I can't get the inital starting text any other quick way and this works perfectly fine
        {
            yield return new WaitForFixedUpdate();
            text.text = TextForValue(slider.value);
            yield return new WaitForSeconds(0.1f);//if the first one is too fast, don't yell at me pls
            text.text = TextForValue(slider.value);
        }

        private void OnChange(TextSlider _, float val)
        {
            text.text = TextForValue(slider.value);
            if (isInt)
                onChange?.Invoke((int)Math.Round(slider.value));
            else
                onChange?.Invoke(slider.value);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }
        public void ApplyValue()
        {
            if (associatedValue != null)
                if (isInt)
                    associatedValue.SetValue((int)Math.Round(slider.value));
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
                return formatter == null ? ((int)Math.Round(value)).ToString() : (formatter.Invoke((int)Math.Round(value)) as string);
            return formatter == null ? value.ToString("N2") : (formatter.Invoke(value) as string);
        }
    }
}
