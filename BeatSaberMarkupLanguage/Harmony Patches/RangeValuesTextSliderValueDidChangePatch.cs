using System.Linq;
using HarmonyLib;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
	[HarmonyPatch(typeof(RangeValuesTextSlider), nameof(RangeValuesTextSlider.HandleNormalizedValueDidChange), MethodType.Normal)]
	public class RangeValuesTextSliderValueDidChangePatch
	{
		private static BasicUIAudioManager _basicUIAudioManager;
		
		public static void Postfix()
		{
			if (_basicUIAudioManager == null)
			{
				_basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().First();
			}
			_basicUIAudioManager.HandleButtonClickEvent();
		}
	}
}