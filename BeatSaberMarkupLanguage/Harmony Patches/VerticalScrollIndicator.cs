using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// This patch fixes the handle position calculation in <see cref="VerticalScrollIndicator.RefreshHandle"/>.
    /// </summary>
    [HarmonyPatch(typeof(VerticalScrollIndicator), nameof(VerticalScrollIndicator.RefreshHandle))]
    internal class VerticalScrollIndicator_RefreshHandle
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)

                // make the minimum size delta of the scroll indicator handle 2 instead of 10
                .MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 10f))
                .SetOperandAndAdvance(2f)

                // match `(1f - _normalizedPageHeight) * num` and replace with `CalculateScrollDistance(this, size)`
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldc_R4, 1f),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Mul),
                    new CodeMatch(OpCodes.Ldloc_0))
                .RemoveInstructions(6)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, 1), // result of ((RectTransform)transform).rect
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(VerticalScrollIndicator_RefreshHandle), nameof(CalculateScrollDistance))))
                .InstructionEnumeration();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CalculateScrollDistance(VerticalScrollIndicator __instance, Rect rect)
        {
            return rect.size.y - __instance._handle.sizeDelta.y - (__instance._padding * 2);
        }
    }
}
