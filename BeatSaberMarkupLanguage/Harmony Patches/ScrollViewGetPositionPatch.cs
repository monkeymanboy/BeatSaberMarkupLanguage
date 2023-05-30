using HarmonyLib;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ScrollView), nameof(ScrollView.position), MethodType.Getter)]
    internal class ScrollViewGetPositionPatch
    {
        /*
         * The bug in question:
         *
         * ScrollView has a "position" property, which is used in TableView.GetVisibleCellsIdRange.
         * The property in question interrogates the anchored X position of the transform.
         *
         * The problem comes from the fact that, in a horizontal list, the anchored X position is often negative,
         * and TableView.GetVisibleCellsIdRange expects positive numbers only, so the returned range is (0, 0).
         *
         * So to fix this, we need to simply invert the "position" property on horizontal lists.
         */
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            var codes = original.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                // We're using a bit of a cheat here, and piggy backing off the first "ret" command, which is only called
                // on Horizontal lists.
                if (codes[i].opcode == OpCodes.Ret)
                {
                    codes.InsertRange(i, new[] {
                        // Loads -1f onto the stack
                        new CodeInstruction(OpCodes.Ldc_R4, -1f),
                        // Multiplies the top two numbers on the stack (-1, and our anchored X coordinate)
                        new CodeInstruction(OpCodes.Mul)
                    });
                    // We don't need to do this on the second "ret" command, which only executes on Vertical lists.
                    break;
                }
            }
            return codes;
        }
    }
}
