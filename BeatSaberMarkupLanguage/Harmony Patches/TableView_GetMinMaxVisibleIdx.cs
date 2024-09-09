using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch]
    internal class TableView_GetMinMaxVisibleIdx
    {
        private static readonly FieldInfo TableTypeField = AccessTools.DeclaredField(typeof(TableView), nameof(TableView._tableType));
        private static readonly FieldInfo ScrollViewField = AccessTools.DeclaredField(typeof(TableView), nameof(TableView._scrollView));
        private static readonly MethodInfo PositionPropertyGetter = AccessTools.DeclaredPropertyGetter(typeof(ScrollView), nameof(ScrollView.position));

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.DeclaredMethod(typeof(TableView), "GetMinVisibleIdx");
            yield return AccessTools.DeclaredMethod(typeof(TableView), "GetMaxVisibleIdx");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(TableTypeField)),
                    new CodeMatch(i => i.Branches(out Label? _)),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(ScrollViewField)),
                    new CodeMatch(i => i.Calls(PositionPropertyGetter)),
                    new CodeMatch(OpCodes.Neg),
                    new CodeMatch(i => i.Branches(out Label? _)))
                .ThrowIfInvalid("Ternary operator not found")
                .RemoveInstructions(8)
                .InstructionEnumeration();
        }
    }
}
