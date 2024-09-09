using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Harmony_Patches;
using BeatSaberMarkupLanguage.Util;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using TMPro;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

[assembly: InternalsVisibleTo("BSML.BeatmapEditor", AllInternalsVisible = true)]

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static readonly string[] FontNamesToRemove = { "NotoSansJP-Medium SDF", "NotoSansKR-Medium SDF", "SourceHanSansCN-Bold-SDF-Common-1(2k)", "SourceHanSansCN-Bold-SDF-Common-2(2k)", "SourceHanSansCN-Bold-SDF-Uncommon(2k)" };

        internal static Config Config { get; private set; }

        [Init]
        public void Init(Conf conf, IPALogger logger)
        {
            Logger.Log = logger;

            try
            {
                Harmony harmony = new("com.monkeymanboy.BeatSaberMarkupLanguage");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Failed to apply Harmony patches\n{ex}");
            }

            Config = conf.Generated<Config>();
        }

        [OnStart]
        public void OnStart()
        {
            LoadAndSetUpFontFallbacksAsync().ContinueWith((task) => Logger.Log.Error($"Failed to set up fallback fonts\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        [OnExit]
        public void OnExit()
        {
        }

        private async Task LoadAndSetUpFontFallbacksAsync()
        {
            await FontManager.AsyncLoadSystemFonts();
            await MainSystemInitAwaiter.WaitForMainSystemInitAsync();

            if (!FontManager.TryGetTMPFontByFullName("Segoe UI", out TMP_FontAsset fallback) &&
                !FontManager.TryGetTMPFontByFamily("Arial", out fallback))
            {
                Logger.Log.Error("Could not find fonts for either Segoe UI or Arial to set up fallbacks");
                return;
            }

            Logger.Log.Debug("Waiting for default font presence");

            await MainMenuAwaiter.WaitForMainMenuAsync();

            Logger.Log.Debug("Setting up default font fallbacks");

            // remove built-in fallback fonts to avoid inconsistencies between CJK characters
            TMP_FontAsset mainTextFont = BeatSaberUI.MainTextFont;
            mainTextFont.fallbackFontAssets.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            mainTextFont.fallbackFontAssetTable.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            mainTextFont.fallbackFontAssetTable.Add(fallback);
            mainTextFont.boldSpacing = 2.2f; // default bold spacing is rather  w i d e
        }
    }
}
