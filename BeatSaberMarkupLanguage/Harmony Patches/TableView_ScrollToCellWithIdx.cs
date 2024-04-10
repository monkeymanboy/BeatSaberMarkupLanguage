using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// This fixes a base-game bug where <see cref="TableView.ScrollToCellWithIdx"/> does not work as expected when called with <see cref="TableView.ScrollPositionType.End"/>.
    /// </summary>
    [HarmonyPatch(typeof(TableView), nameof(TableView.ScrollToCellWithIdx))]
    internal class TableView_ScrollToCellWithIdx
    {
        private static readonly FieldInfo _scrollViewField = AccessTools.DeclaredField(typeof(TableView), nameof(TableView._scrollView));
        private static readonly FieldInfo _cellSizeField = AccessTools.DeclaredField(typeof(TableView), nameof(TableView._cellSize));
        private static readonly MethodInfo _scrollPageSizeGetter = AccessTools.DeclaredPropertyGetter(typeof(ScrollView), nameof(ScrollView.scrollPageSize));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            return new CodeMatcher(codeInstructions, ilGenerator)

                // find the content of the `case ScrollPositionType.End:` statement
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Blt),
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Starg_S))
                .ThrowIfInvalid("ScrollPositionType.End case not found")
                .RemoveInstructions(9) // remove it all

                // find the value that is passed to `ScrollTo`
                .MatchForward(
                    true,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(_scrollViewField)),
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Conv_R4),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(_cellSizeField)),
                    new CodeMatch(OpCodes.Mul))
                .ThrowIfInvalid("(idx * _cellSize) not found")
                .Advance(1) // skip over mul
                .CreateLabel(out Label label) // create label so we can skip if scrollPositionType != ScrollPositionType.End
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_2), // scrollPositionType
                    new CodeInstruction(OpCodes.Ldc_I4_2), // ScrollPositionType.End
                    new CodeInstruction(OpCodes.Bne_Un_S, label),
                    new CodeInstruction(OpCodes.Ldarg_0), // load `this`
                    new CodeInstruction(OpCodes.Ldfld, _scrollViewField),
                    new CodeInstruction(OpCodes.Call, _scrollPageSizeGetter),
                    new CodeInstruction(OpCodes.Sub)) // do `(idx * _cellSize) - scrollPageSize`
                .InstructionEnumeration();
        }
    }
}
