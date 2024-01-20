using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Util;
using HarmonyLib;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch]
    internal static class MainSystemInitAwaiter
    {
        private static TaskCompletionSource<VoidResult> taskCompletionSource = new();

        public static Task WaitForMainSystemInitAsync() => taskCompletionSource.Task;

        [HarmonyPatch(typeof(MainSystemInit), nameof(MainSystemInit.Init))]
        [HarmonyPostfix]
        private static void OnInit()
        {
            taskCompletionSource.SetResult(default);
        }

        [HarmonyPatch(typeof(AppInit), "OnDestroy")]
        [HarmonyPostfix]
        private static void OnDestroy()
        {
            taskCompletionSource = new();
        }
    }
}
