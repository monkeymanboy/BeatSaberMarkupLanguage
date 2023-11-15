using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BeatSaberMarkupLanguage.Components;
using HarmonyLib;
using TMPro;

#if !GAME_VERSION_1_29_0
using UnityEngine;
#endif

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
#if !GAME_VERSION_1_29_0
    /// <summary>
    /// This patch copies the new <see cref="Material.enabledKeywords"/> since the version of TextMesh Pro that Beat Saber uses doesn't support it (yet).
    /// </summary>
    [HarmonyPatch(typeof(TMP_Text), "CreateMaterialInstance")]
    internal static class TMP_Text_CreateMaterialInstance
    {
        public static void Postfix(Material source, ref Material __result)
        {
            __result.enabledKeywords = source.enabledKeywords;
        }
    }
#endif // !GAME_VERSION_1_29_0

    [HarmonyPatch(typeof(TMP_Text), "CalculatePreferredValues")]
    internal static class TMP_Text_CalculatePreferredValues
    {
        private static readonly MethodInfo TargetMethod = AccessTools.Method(typeof(char), nameof(char.IsWhiteSpace), new[] { typeof(char) });
        private static readonly MethodInfo OverrideMethod = AccessTools.Method(typeof(TMP_Text_CalculatePreferredValues), nameof(IsWhiteSpace));

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
