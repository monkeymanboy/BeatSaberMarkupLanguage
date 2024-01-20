using System.Linq;
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
                ShaderUtilities.k_ShaderRef_MobileSDF = Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "TextMeshPro/Mobile/Distance Field");
            }

            __result = ShaderUtilities.k_ShaderRef_MobileSDF;

            return false;
        }
    }

    [HarmonyPatch(typeof(TMP_Settings), nameof(TMP_Settings.instance), MethodType.Getter)]
    internal static class TMP_Settings_GetFontAsset
    {
        public static void Postfix()
        {
            if (TMP_Settings.s_Instance.m_defaultFontAsset == null)
            {
                TMP_Settings.s_Instance.m_defaultFontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(f => f.name == "Teko-Medium SDF");
            }
        }
    }
}
