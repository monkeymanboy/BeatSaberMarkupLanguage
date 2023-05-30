using System;
using System.Collections.Generic;
using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    // TODO: try and remove need for this
    // this is needed for tab pages so that when you change pages nothing is selected
    [HarmonyPatch(typeof(SegmentedControl), "HandleCellSelectionDidChange", new Type[] { typeof(SelectableCell), typeof(SelectableCell.TransitionType), typeof(object) })]
    internal class SegmentedControlCellSelectionStateDidChange
    {
        private static bool Prefix(SelectableCell selectableCell, SelectableCell.TransitionType transitionType, object changeOwner, ref int ____selectedCellNumber, Action<SegmentedControl, int> ___didSelectCellEvent, Dictionary<int, Action<int>> ____callbacks, SegmentedControl __instance)
        {
            if (____selectedCellNumber == -1)
            {
                SegmentedControlCell segmentedControlCell = (SegmentedControlCell)selectableCell;
                ____selectedCellNumber = segmentedControlCell.cellNumber;
                ___didSelectCellEvent?.Invoke(__instance, segmentedControlCell.cellNumber);
                if (____callbacks.TryGetValue(segmentedControlCell.cellNumber, out Action<int> value))
                {
                    value?.Invoke(segmentedControlCell.cellNumber);
                }

                return false;
            }

            return true;
        }
    }
}
