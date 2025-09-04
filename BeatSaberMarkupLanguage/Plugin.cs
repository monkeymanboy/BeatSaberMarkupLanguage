using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Util;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using TMPro;
using UnityEngine.TextCore.LowLevel;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

[assembly: InternalsVisibleTo("BSML.BeatmapEditor", AllInternalsVisible = true)]

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        // All of these fonts are included by default with Windows 10+ https://learn.microsoft.com/en-us/typography/fonts/windows_10_font_list
        // Older versions of Windows only include a subset of these fonts, so some Unicode characters may not show up properly in-game.
        private static readonly FontManager.TMPFontCreationArgs[] FontNamesToLoad =
        [
            new("Segoe UI"),
            new("Segoe UI Emoji", RenderMode: GlyphRenderMode.COLOR),
            new("Segoe UI Symbol"),
            new("Segoe UI Historic"),
            new("Microsoft Sans Serif"),
            new("Microsoft Himalaya"),
            new("Microsoft JhengHei UI"),
            new("Microsoft New Tai Lue"),
            new("Microsoft PhagsPa"),
            new("Microsoft Tai Le"),
            new("Microsoft YaHei UI"),
            new("Microsoft Yi Baiti"),
            new("Gadugi"),
            new("Nirmala UI"),
            new("Malgun Gothic"),
            new("SimSun"),
        ];

        private static readonly string[] FontNamesToRemove = ["NotoSansJP-Medium SDF", "NotoSansKR-Medium SDF", "SourceHanSansCN-Bold-SDF-Common-1(2k)", "SourceHanSansCN-Bold-SDF-Common-2(2k)", "SourceHanSansCN-Bold-SDF-Uncommon(2k)"];

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
            await MainMenuAwaiter.WaitForMainMenuAsync();

            Logger.Log.Debug("Setting up default font fallbacks");

            ProcessFont(BeatSaberUI.MainTextFont, false);
            ProcessFont(BeatSaberUI.MonochromeTextFont, true);
        }

        private void ProcessFont(TMP_FontAsset fontAsset, bool monochrome)
        {
            // remove built-in fallback fonts to avoid inconsistencies between CJK characters
            fontAsset.fallbackFontAssets.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            fontAsset.fallbackFontAssetTable.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            fontAsset.fallbackFontAssetTable.AddRange(FontManager.CreateFallbackFonts(FontNamesToLoad, monochrome));

            // default bold spacing is rather  w i d e
            fontAsset.boldSpacing = 2.2f;
        }
    }
}
