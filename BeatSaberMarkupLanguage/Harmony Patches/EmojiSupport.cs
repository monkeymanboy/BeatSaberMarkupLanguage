using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch]
    internal class EmojiSupport
    {
        [HarmonyPatch(typeof(TMP_FontAsset), nameof(TMP_FontAsset.CreateFontAssetInstance))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TMP_FontAsset_CreateFontAssetInstance_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldstr, "TextMeshPro/Sprite"), // This shader is not included in the game build.
                    new CodeMatch(i => i.Calls(AccessTools.DeclaredMethod(typeof(Shader), nameof(Shader.Find)))),
                    new CodeMatch(OpCodes.Newobj))
                .SetAndAdvance(OpCodes.Nop, null)
                .SetAndAdvance(OpCodes.Nop, null)
                .SetAndAdvance(OpCodes.Call, AccessTools.DeclaredMethod(typeof(EmojiSupport), nameof(GetSpriteMaterial)))
                .InstructionEnumeration();
        }

        private static Material GetSpriteMaterial()
        {
            // This is the material used for ImageViews. It won't support all of TMP's features but curves correctly.
            return new Material(Utilities.ImageResources.NoGlowMat)
            {
                name = "TextMeshPro Sprite No Glow",
                mainTexture = null,
            };
        }
    }
}
