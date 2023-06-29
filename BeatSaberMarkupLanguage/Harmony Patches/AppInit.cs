using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities.Async;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(AppInit), nameof(AppInit.Start))]
    internal static class AppInit_Start
    {
        private static readonly string[] FontNamesToRemove = { "NotoSansJP-Medium SDF", "NotoSansKR-Medium SDF", "SourceHanSansCN-Bold-SDF-Common-1(2k)", "SourceHanSansCN-Bold-SDF-Common-2(2k)", "SourceHanSansCN-Bold-SDF-Uncommon(2k)" };

        private static void Postfix(AppInit __instance)
        {
            if (__instance.GetAppStartType() != AppInit.AppStartType.AppStart)
            {
                return;
            }

            FontManager.AsyncLoadSystemFonts().ContinueWith(
                task =>
                {
                    if (!FontManager.TryGetTMPFontByFullName("Segoe UI", out TMP_FontAsset fallback) &&
                        !FontManager.TryGetTMPFontByFamily("Arial", out fallback))
                    {
                        Logger.Log.Warn("Could not find fonts for either Segoe UI or Arial to set up fallbacks");
                        return;
                    }

                    IEnumerator SetupFont()
                    {
                        yield return new WaitUntil(() => BeatSaberUI.MainTextFont != null);
                        Logger.Log.Debug("Setting up default font fallbacks");

                        // remove built-in fallback fonts to avoid inconsistencies between CJK characters
                        BeatSaberUI.MainTextFont.fallbackFontAssets.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
                        BeatSaberUI.MainTextFont.fallbackFontAssetTable.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
                        BeatSaberUI.MainTextFont.fallbackFontAssetTable.Add(fallback);
                    }

                    Logger.Log.Debug("Waiting for default font presence");
                    if (fallback != null)
                    {
                        BeatSaberUI.CoroutineStarter.StartCoroutine(SetupFont());
                    }
                },
                UnityMainThreadTaskScheduler.Default)
            .ContinueWith(
                t =>
                {
                    Logger.Log.Error($"Failed to set up fallback fonts\n{t.Exception}");
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
