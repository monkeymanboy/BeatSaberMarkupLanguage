using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// This patch forces a minimum vertical size on text objects since if it's too small
    /// vertically for one or more characters, the text mesh won't update at all.
    /// </summary>
    [HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable", MethodType.Normal)]
    internal static class TextMeshProUGUI_OnEnable
    {
        // this value is eyeballed
        private const float fontSizeFactor = 1.75f;

        // text rendering only messes up with certain overflow modes
        private static readonly TextOverflowModes[] textOverflowModes = new[] { TextOverflowModes.Truncate, TextOverflowModes.Ellipsis };

        public static void Postfix(TextMeshProUGUI __instance)
        {
            if (!textOverflowModes.Contains(__instance.overflowMode)) return;

            Vector2 sizeDelta = __instance.rectTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(sizeDelta.y, fontSizeFactor * __instance.fontSize);
            __instance.rectTransform.sizeDelta = sizeDelta;
        }
    }
}
