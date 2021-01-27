using HarmonyLib;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(HorizontalOrVerticalLayoutGroup), "SetChildrenAlongAxis")]
    class HorizontalOrVerticalLayoutGroup_SetChildrenAlongAxis
    {
        static void Prefix(HorizontalOrVerticalLayoutGroup __instance)
        {
            __instance.CalculateLayoutInputHorizontal();
        }
    }
}