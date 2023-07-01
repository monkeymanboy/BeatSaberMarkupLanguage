using HarmonyLib;
using Zenject;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(SceneContext), nameof(SceneContext.Awake))]
    internal static class SceneContext_Awake
    {
        private static void Prefix(SceneContext __instance)
        {
            if (__instance.name != "SceneContext" || __instance.gameObject.scene.name != "MainMenu")
            {
                return;
            }

            // PostInstall here happens right after all bindings are installed but before anything else in the scene loads.
            // This is (unfortunately) necessary because some mods currently parse their files before Zenject calls IInitializables.
            __instance.PostInstall += BSMLParser.instance.SetUpTags;
        }
    }
}
