using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
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
}
