using System.Linq;
using HarmonyLib;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(ModalView), nameof(ModalView.Show))]
    internal static class FixModalBlockerLayer
    {
        private static void Postfix(ModalView __instance, GameObject ____blockerGO)
        {
            Canvas blockerCanvas = ____blockerGO.GetComponent<Canvas>();
            HMUI.Screen screen = __instance.transform.parent.GetComponentInParent<HMUI.Screen>();
            int sortingOrder = screen != null
                ? screen
                    .GetComponentsInChildren<Canvas>()
                    .Where(x => x.sortingLayerID == blockerCanvas.sortingLayerID)
                    .Select(x => x.sortingOrder)
                    .DefaultIfEmpty(0)
                    .Max() + 1
                : 1;

            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingOrder = sortingOrder;

            blockerCanvas = __instance.GetComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingOrder = sortingOrder + 1;
        }
    }
}
