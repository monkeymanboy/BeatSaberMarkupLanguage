using HarmonyLib;
using HMUI;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    /// <summary>
    /// This patch simply adds a default value to the <see cref="TableView._padding" /> field.
    /// </summary>
    [HarmonyPatch(typeof(TableView), MethodType.Constructor)]
    internal class TableView_ctor
    {
        private static void Prefix(TableView __instance)
        {
            __instance._padding = new FloatRectOffset() { left = 0, top = 0, right = 0, bottom = 0 };
        }
    }
}
