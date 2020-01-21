using Harmony;
using HMUI;
using System;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    //todo: try and remove need for this
    //this is needed for tab pages so that when you change pages nothing is selected
    
    [HarmonyPatch(typeof(SegmentedControl), "CellSelectionStateDidChange",
        new Type[] { typeof(SegmentedControlCell) })]
    class SegmentedControlCellSelectionStateDidChange
    {
        static bool Prefix(SegmentedControlCell changedCell, ref int ____selectedCellNumber, Action<SegmentedControl, int> ___didSelectCellEvent, SegmentedControl __instance)
        {
            if (____selectedCellNumber == -1)
            {
                ____selectedCellNumber = changedCell.cellNumber;
                ___didSelectCellEvent?.Invoke(__instance, changedCell.cellNumber);
                return false;
            }
            return true;
        }
    }
}
