using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BeatSaberMarkupLanguage.MenuButtons;
using HarmonyLib;
using HMUI;
using Zenject;

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

    /// <summary>
    /// This patch adds <see cref="MenuButtonsViewController"/> as the argument passed to <see cref="FlowCoordinator.ProvideInitialViewControllers(ViewController, ViewController, ViewController, ViewController, ViewController)"/>'s <c>leftScreenViewController</c>.
    /// </summary>
    [HarmonyPatch(typeof(MainFlowCoordinator), "DidActivate")]
    internal class MainFlowCoordinator_DidActivate
    {
        private static readonly MethodInfo ProvideInitialViewControllersMethod = AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.ProvideInitialViewControllers));
        private static readonly MethodInfo BeatSaberUIDiContainerGetter = AccessTools.PropertyGetter(typeof(BeatSaberUI), nameof(BeatSaberUI.DiContainer));
        private static readonly MethodInfo DiContainerResolveViewController = AccessTools.Method(typeof(DiContainer), nameof(DiContainer.Resolve), generics: new[] { typeof(MenuButtonsViewController) });

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldnull), // leftScreenViewController
                    new CodeMatch(OpCodes.Ldnull), // rightScreenViewController
                    new CodeMatch(OpCodes.Ldnull), // bottomScreenViewController
                    new CodeMatch(OpCodes.Ldnull), // topScreenViewController
                    new CodeMatch(i => i.Calls(ProvideInitialViewControllersMethod)))
                .ThrowIfInvalid($"Call to {nameof(FlowCoordinator.ProvideInitialViewControllers)} not found")
                .RemoveInstruction()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, BeatSaberUIDiContainerGetter),
                    new CodeInstruction(OpCodes.Call, DiContainerResolveViewController))
                .InstructionEnumeration();
        }
    }

    /// <summary>
    /// This patch passes <see cref="MenuButtonsViewController"/> to <see cref="FlowCoordinator.SetLeftScreenViewController(ViewController, ViewController.AnimationType)"/> instead of <see langword="null" />.
    /// </summary>
    [HarmonyPatch(typeof(MainFlowCoordinator), "TopViewControllerWillChange")]
    internal class MainFlowCoordinator_TopViewControllerWillChange
    {
        private static readonly MethodInfo SetLeftScreenViewControllerMethod = AccessTools.Method(typeof(FlowCoordinator), nameof(FlowCoordinator.SetLeftScreenViewController));
        private static readonly FieldInfo ProvidedLeftScreenViewControllerField = AccessTools.Field(typeof(FlowCoordinator), nameof(FlowCoordinator._providedLeftScreenViewController));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                /* there are two calls to SetLeftScreenViewController - the first one is for when newViewController == _mainMenuViewController is true */
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Brfalse), // newViewController == _mainMenuViewController
                    new CodeMatch(OpCodes.Ldarg_0), // this
                    new CodeMatch(OpCodes.Ldnull), // viewController
                    new CodeMatch(OpCodes.Ldarg_3), // animationType
                    new CodeMatch(i => i.Calls(SetLeftScreenViewControllerMethod)))
                .ThrowIfInvalid($"Call to {nameof(FlowCoordinator.SetLeftScreenViewController)} not found")
                .Advance(2)
                .RemoveInstruction() // ldnull
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, ProvidedLeftScreenViewControllerField)) // this field is set by ProvideInitialViewControllers in the DidActivate patch above
                .InstructionEnumeration();
        }
    }
}
