using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BeatSaberMarkupLanguage.Components;
using HarmonyLib;
using TMPro;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(TextMeshProUGUI), "GenerateTextMesh")]
    internal static class TextMeshProUGUI_GenerateTextMesh
    {
        private static readonly MethodInfo TargetMethod = AccessTools.Method(typeof(char), nameof(char.IsWhiteSpace), new[] { typeof(char) });
        private static readonly MethodInfo OverrideMethod = AccessTools.Method(typeof(TextMeshProUGUI_GenerateTextMesh), nameof(IsWhiteSpace));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            foreach (CodeInstruction codeInstruction in codeInstructions)
            {
                if (codeInstruction.operand is MethodInfo methodInfo && methodInfo == TargetMethod)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, OverrideMethod);
                }
                else
                {
                    yield return codeInstruction;
                }
            }
        }

        private static bool IsWhiteSpace(char c, TMP_Text instance) => instance is not WhitespaceIncludingCurvedTextMeshPro && char.IsWhiteSpace(c);
    }
}
