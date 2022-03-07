using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
	[HarmonyPatch(typeof(RangeValuesTextSlider), nameof(RangeValuesTextSlider.HandleNormalizedValueDidChange), MethodType.Normal)]
	public class RangeValuesTextSliderValueDidChangePatch
	{
		public static void Postfix() => BeatSaberUI.BasicUIAudioManager.HandleButtonClickEvent();
	}
}