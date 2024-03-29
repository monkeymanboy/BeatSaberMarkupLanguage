using BeatSaberMarkupLanguage.Components;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ScrollView), nameof(ScrollView.SetContentSize))]
    internal class ScrollViewSetContentSize
    {
        public static void Postfix(ScrollView __instance)
        {
            if (__instance is BSMLScrollView bsmlScrollView)
            {
                bsmlScrollView.UpdateViewport();
            }
        }
    }
}
