using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage.MenuButtons;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// Since we add the <see cref="MenuButtonsViewController"/> on the left side of the main menu, the default horizontal animation makes the center view controller clip our view controller.
    /// This forces the in/out animations of the <see cref="MainFlowCoordinator"/> to <see cref="ViewController.AnimationDirection.Vertical"/> to avoid this.
    /// </summary>
    [HarmonyPatch]
    internal class FlowCoordinator_PresentFlowCoordinator
    {
        private static void Prefix(FlowCoordinator __instance, ref ViewController.AnimationDirection animationDirection)
        {
            if (__instance is MainFlowCoordinator)
            {
                animationDirection = ViewController.AnimationDirection.Vertical;
            }
        }

        private static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.PresentFlowCoordinator));
            yield return AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.DismissFlowCoordinator));
            yield return AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.PresentViewController));
            yield return AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.DismissViewController));
        }
    }
}
