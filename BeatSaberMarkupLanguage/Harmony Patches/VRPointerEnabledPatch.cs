using System;
using HarmonyLib;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(VRPointer), nameof(VRPointer.EnabledLastSelectedPointer), MethodType.Normal)]
    internal class VRPointerEnabledPatch
    {
        /*
        * The bug in question:
        *
        * VRPointer now get initialized again on GameScene, rendering an old reference for FloatingScreens useless
        * To fix this, we subscribe to an event for when a VRPointer is enabled (which happens when it is being used)
        * and update this reference for the FloatingScreen.
        *
        */

        public static event Action<VRPointer> PointerEnabled;

        public static void Postfix(VRPointer __instance)
        {
            PointerEnabled?.Invoke(__instance);
        }
    }
}
