using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HMUI;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    [HarmonyPatch(typeof(CustomFormatRangeValuesSlider), "TextForValue")]
    internal static class ApplyCustomSliderTexts
    {
        public static ConditionalWeakTable<RangeValuesTextSlider, SliderSetting> remappers = new ConditionalWeakTable<RangeValuesTextSlider, SliderSetting>();

        private static bool Prefix(RangeValuesTextSlider __instance, float value, ref string __result)
        {
            if (!remappers.TryGetValue(__instance, out var sliderSetting))
            {
                return true;
            }

            __result = sliderSetting.TextForValue(value);
            return false;
        }
    }

    public class SliderSetting : GenericSliderSetting
    {
        public bool isInt = false;
        public float increments;
        public float Value
        {
            get => slider.value;
            set => slider.value = value;
        }

        public override void Setup()
        {
            base.Setup();

            ApplyCustomSliderTexts.remappers.Add(slider, this);

            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = (int)Math.Round((slider.maxValue - slider.minValue) / increments) + 1;
            ReceiveValue();
            slider.valueDidChangeEvent += OnChange;
        }

        private float lastValue = float.NegativeInfinity;

        private void OnChange(TextSlider _, float val)
        {
            if (isInt)
                val = (int)Math.Round(val);

            if (lastValue == val)
                return;

            lastValue = val;

            if (isInt)
                onChange?.Invoke((int)val);
            else
                onChange?.Invoke(val);

            if (updateOnChange)
                ApplyValue();
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
        }

        internal string TextForValue(float value)
        {
            if (isInt)
                return formatter == null ? ((int)Math.Round(value)).ToString() : (formatter.Invoke((int)Math.Round(value)) as string);
            else
                return formatter == null ? value.ToString("N2") : (formatter.Invoke(value) as string);
        }
    }
}
