using System.Linq;
using HarmonyLib;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ModalView), nameof(ModalView.Show))]
    public static class FixModalBlockerLayer
    {
        private static void Postfix(ModalView __instance, GameObject ____blockerGO)
        {
            var cb = ____blockerGO.GetComponent<Canvas>();

            var h = (__instance.transform.parent.GetComponentInParent<HMUI.Screen>()
                ?.GetComponentsInChildren<Canvas>()
                .Where(x => x.sortingLayerID == cb.sortingLayerID)
                .Select(x => x.sortingOrder)
                .DefaultIfEmpty(0)
                .Max() ?? 0) + 1;

            cb.overrideSorting = true;
            cb.sortingOrder = h;

            cb = __instance.GetComponent<Canvas>();
            cb.overrideSorting = true;
            cb.sortingOrder = h + 1;
        }
    }
}
