using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class SliderSetting : GenericSliderSetting
    {
        public bool isInt = false;
        public float increments;

        public override void Setup()
        {
            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = (int)Math.Round((slider.maxValue - slider.minValue) / increments) + 1;
            ReceiveValue();
            slider.valueDidChangeEvent += OnChange;
            if (isActiveAndEnabled)
                StartCoroutine(SetInitialText());
        }

        // I don't really like this but for some reason I can't get the initial starting text any other quick way and this works perfectly fine
        protected override IEnumerator SetInitialText()
        {
            yield return new WaitForFixedUpdate();
            text.text = TextForValue(slider.value);
            yield return new WaitForSeconds(0.1f); // If the first one is too fast, don't yell at me pls
            text.text = TextForValue(slider.value);
        }

        protected override void RaiseValueChanged(bool emitEvent)
        {
            text.text = TextForValue(slider.value); // Update text no matter what.
            if (emitEvent)
            {
                // Logger.log?.Debug($"RaiseValueChanged ({slider.value}): IsDragging: {IsDragging} | updateDuringDrag: {updateDuringDrag}");
                if (isInt)
                    onChange?.Invoke((int)Math.Round(slider.value));
                else
                    onChange?.Invoke(slider.value);

                if (updateOnChange)
                    ApplyValue();
            }
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
            {
                if (isInt)
                    associatedValue.SetValue((int)Math.Round(slider.value));
                else
                    associatedValue.SetValue(slider.value);
            }
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                slider.value = isInt ? (int)associatedValue.GetValue() : (float)associatedValue.GetValue();

            text.text = TextForValue(slider.value);
        }

        protected string TextForValue(float value)
        {
            if (isInt)
                return formatter == null ? ((int)Math.Round(value)).ToString() : (formatter.Invoke((int)Math.Round(value)) as string);
            else
                return formatter == null ? value.ToString("N2") : (formatter.Invoke(value) as string);
        }
    }
}
