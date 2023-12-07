using HarmonyLib;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(LevelListTableCell), nameof(LevelListTableCell.RefreshVisuals))]
    internal static class LevelListTableCell_RefreshVisuals
    {
        private static void Postfix(LevelListTableCell __instance)
        {
            RectTransform rectTransform = __instance._songNameText.rectTransform;
            Vector2 offsetMin = rectTransform.offsetMin;
            Vector2 offsetMax = rectTransform.offsetMax;
            offsetMax.y = offsetMin.y + 6.5f; // by default sizeDelta.y is 5.74 which is _slightly_ too small
            rectTransform.offsetMax = offsetMax;
        }
    }
}
