using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ImageView), nameof(ImageView.GenerateFilledSprite))]
    internal static class ImageViewFilledImagePatch
    {
        // This is Beat Games' incorrect AddQuad method, which completely forgets about the curvedUIRadius parameter.
        private static readonly MethodInfo IncorrectQuadOverload = AccessTools.Method(
            typeof(ImageView),
            nameof(ImageView.AddQuad),
            new Type[] { typeof(VertexHelper), typeof(Vector3[]), typeof(Color32), typeof(Vector3[]) });

        // MethodInfo to our correct AddQuad method; see below for implementation
        private static readonly MethodInfo CorrectQuadOverload = SymbolExtensions.GetMethodInfo(() => AddQuad(null, null, new Color32(0, 0, 0, 0), null, 0));

        // This code loads the "curvedUIRadius" field, and then calls the correct AddQuad method.
        private static readonly List<CodeInstruction> ReplacementCodeInstructions = new()
        {
            new CodeInstruction(OpCodes.Ldarg_3),
            new CodeInstruction(OpCodes.Call, CorrectQuadOverload),
        };

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                // Did we find an instance of the old, incorrect method?
                if ((instruction.operand as MethodInfo) == IncorrectQuadOverload)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Call, CorrectQuadOverload);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // This is our custom AddQuad method, which correctly applies curvature to the created vertices.
        private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs, float curvedUIRadius)
        {
            int currentVertCount = vertexHelper.currentVertCount;
            Vector2 uv = new(curvedUIRadius, 0f);
            for (int i = 0; i < 4; i++)
            {
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i], Vector2.zero, uv, Vector2.zero, Vector3.zero, Vector4.zero);
            }

            vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }
    }
}
