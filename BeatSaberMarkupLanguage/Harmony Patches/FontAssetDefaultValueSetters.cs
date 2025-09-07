using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ShaderUtilities), nameof(ShaderUtilities.ShaderRef_MobileSDF), MethodType.Getter)]
    internal static class ShaderUtilities_ShaderRef_MobileSDF
    {
        public static bool Prefix(ref Shader __result)
        {
            if (ShaderUtilities.k_ShaderRef_MobileSDF == null)
            {
                ShaderUtilities.k_ShaderRef_MobileSDF = Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(s => s.name == "TextMeshPro/Mobile/Distance Field");
            }

            __result = ShaderUtilities.k_ShaderRef_MobileSDF;

            return false;
        }
    }

    [HarmonyPatch]
    internal static class TextMeshPro_LoadFontAsset
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.DeclaredMethod(typeof(TextMeshPro), "LoadFontAsset");
            yield return AccessTools.DeclaredMethod(typeof(TextMeshProUGUI), "LoadFontAsset");
        }

        public static void Prefix(TMP_Text __instance)
        {
            if (__instance.font == null)
            {
                __instance.font = BeatSaberUI.MainTextFont;
            }
        }
    }
}
