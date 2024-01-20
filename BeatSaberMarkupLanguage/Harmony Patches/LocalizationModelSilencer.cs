using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BGLib.Polyglot;
using HarmonyLib;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(LocalizationModel))]
    internal static class LocalizationModelSilencer
    {
        private static readonly MethodInfo UnityDebugLogWarning = AccessTools.DeclaredMethod(typeof(Debug), nameof(Debug.LogWarning), new Type[] { typeof(object) });

        [HarmonyPatch(nameof(LocalizationModel.Get))]
        [HarmonyPatch(nameof(LocalizationModel.TryGet))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(UnityDebugLogWarning))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
