using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    // Feature to disable thumbstick scrolling in BSMLTabs for users suffering from thumbstick drift
    // who just want to set sliders without them scrolling away
    [HarmonyPatch(typeof(ScrollView), "HandlePointerDidEnter")]
    internal static class ScrollViewHandlePointerDidEnterPatch
    {
        private static void Postfix(ScrollView __instance)
        {
            if (Plugin.config.DisableThumbstickScroll && __instance.GetComponentInParent<GameplaySetupViewController>() != null)
            {
                Traverse.Create(__instance).Field("_isHoveredByPointer").SetValue(false);
            }
        }
    }
}
