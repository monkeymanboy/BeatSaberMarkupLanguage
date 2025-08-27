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
using UnityEngine;
using UnityEngine.TextCore;
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
        private static readonly string[] FontNamesToLoad = ["Segoe UI", "Segoe UI Emoji", "Segoe UI Symbol", "Segoe UI Historic", "Microsoft Sans Serif", "Microsoft Himalaya", "Microsoft JhengHei UI", "Microsoft New Tai Lue", "Microsoft PhagsPa", "Microsoft Tai Le", "Microsoft Uighur", "Microsoft YaHei UI", "Microsoft Yi Baiti", "Gadugi", "Nirmala UI"];
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

            // remove built-in fallback fonts to avoid inconsistencies between CJK characters
            TMP_FontAsset mainTextFont = BeatSaberUI.MainTextFont;
            mainTextFont.fallbackFontAssets.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            mainTextFont.fallbackFontAssetTable.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
            mainTextFont.boldSpacing = 2.2f; // default bold spacing is rather  w i d e

            if (Config.UseColoredEmoji && FontManager.TryGetFontByFullName("Segoe UI Emoji", out Font font))
            {
                TMP_FontAsset emoji = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.COLOR, 4096, 4096);
                emoji.name = "Segoe UI Emoji (Color)";
                FaceInfo faceInfo = emoji.faceInfo;
                faceInfo.scale = 0.85f;
                emoji.faceInfo = faceInfo;

                mainTextFont.fallbackFontAssetTable.Add(emoji);
            }

            mainTextFont.fallbackFontAssetTable.AddRange(FontManager.CreateFallbackFonts(FontNamesToLoad));
        }
    }
}
