using HarmonyLib;
using TMPro;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(TMP_SubMeshUI), "UpdateMaterial")]
    internal class TMP_SubMeshUI_UpdateMaterial
    {
        private static bool Prefix(TMP_SubMeshUI __instance)
        {
            return __instance.textComponent.fontSharedMaterial != null;
        }
    }
}
