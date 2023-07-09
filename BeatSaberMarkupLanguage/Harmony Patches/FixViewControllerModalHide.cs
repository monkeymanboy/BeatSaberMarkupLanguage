using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// This patch makes modal views disable immediately when the parent view controller is deactivated no matter what.
    /// The base game behavior is to only immediately disable them if the screen system is being disabled, which causes the view to stick around if the
    /// view controller is simply being dismissed since the animation coroutine can't complete before the view controller GameObject is disabled.
    /// </summary>
    /// <remarks>
    /// This currently replaces the <c>!screenSystemDisabling</c> passed as the argument for <c>animate</c> in the call to <see cref="ModalView.Hide"/> with just <c>false</c>.
    /// </remarks>
    [HarmonyPatch(typeof(ModalView), nameof(ModalView.HandleParentViewControllerDidDeactivate))]
    internal static class FixViewControllerModalHide
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldarg_2),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Ceq))
                .RemoveInstructions(3)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldc_I4_0))
                .InstructionEnumeration();
        }
    }
}
