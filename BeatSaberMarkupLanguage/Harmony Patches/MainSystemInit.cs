using System;
using HarmonyLib;
using Zenject;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [Obsolete("This patch will be removed along with CoroutineStarter")]
    [HarmonyPatch(typeof(MainSystemInit), nameof(MainSystemInit.InstallBindings))]
    internal static class MainSystemInit_InstallBindings
    {
        private static void Postfix(DiContainer container)
        {
            BeatSaberUI.CoroutineStarter = container.Resolve<ICoroutineStarter>();
        }
    }
}
