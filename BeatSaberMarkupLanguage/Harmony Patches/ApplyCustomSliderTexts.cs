using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Components.Settings;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(CustomFormatRangeValuesSlider), "TextForValue")]
    internal static class ApplyCustomSliderTexts
    {
        public static ConditionalWeakTable<RangeValuesTextSlider, SliderSetting> Remappers = new();

        private static bool Prefix(RangeValuesTextSlider __instance, float value, ref string __result)
        {
            if (!Remappers.TryGetValue(__instance, out SliderSetting sliderSetting))
            {
                return true;
            }

            __result = sliderSetting.TextForValue(value);
            return false;
        }
    }
}
