using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.OpenType;
using IPA.Utilities.Async;
using Microsoft.Win32;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace BeatSaberMarkupLanguage
{
    public static class FontManager
    {
        // path → loaded font object
        private static readonly Dictionary<string, Font> LoadedFontsCache = new();

        // family → list of fonts in family
        private static Dictionary<string, List<FontInfo>> fontInfoLookup;

        // full name → font info
        private static Dictionary<string, FontInfo> fontInfoLookupFullName;

        /// <summary>
        /// Gets the <see cref="Task"/> associated with an ongoing call to <see cref="AsyncLoadSystemFonts"/>.
        /// </summary>
        public static Task SystemFontLoadTask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="FontManager"/> is initialized.
        /// </summary>
        /// <remarks>
        /// You can <see langword="await"/> <see cref="AsyncLoadSystemFonts"/>, or <see cref="SystemFontLoadTask"/> if it is non-null.
        /// When they complete, <see cref="FontManager"/> will be initialized.
        /// </remarks>
        public static bool IsInitialized => fontInfoLookup != null;

        /// <summary>
        /// Asynchronously loads all of the installed system fonts into <see cref="FontManager"/>.
        /// </summary>
        /// <remarks>
        /// Only one of this may be running at a time. If this has already been called, this will simply return the existing task.
        /// If <see cref="FontManager"/> has been initialized, this completes immediately.
        /// </remarks>
        /// <returns>A task representing the async operation.</returns>
        public static Task AsyncLoadSystemFonts()
        {
            if (IsInitialized)
            {
                return Task.CompletedTask;
            }

            if (SystemFontLoadTask != null)
            {
                return SystemFontLoadTask;
            }

            Task<(Dictionary<string, List<FontInfo>> Families, Dictionary<string, FontInfo> Fulls)> task = Task.Factory.StartNew(LoadSystemFonts).Unwrap();
            SystemFontLoadTask = task.ContinueWith(
                t =>
                {
                    Logger.Log.Debug("Font loading complete");
                    Interlocked.CompareExchange(ref fontInfoLookupFullName, t.Result.Fulls, null);
                    Interlocked.CompareExchange(ref fontInfoLookup, t.Result.Families, null);
                    return Task.CompletedTask;
                },
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

            task.ContinueWith(t => Logger.Log.Error($"Font loading errored: {t.Exception}"), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion);
            return SystemFontLoadTask;
        }

        /// <summary>
        /// Adds a specified OpenType file to the font manager for lookup by name.
        /// </summary>
        /// <param name="path">The path to add to the manager.</param>
        /// <returns>the <see cref="Font"/> the file contained.</returns>
        /// <exception cref="ArgumentException">If the file pointed to by <paramref name="path"/> is not an OpenType file or if <paramref name="path"/> is not a valid file path.</exception>
        /// <exception cref="FileNotFoundException">If the file does not exist.</exception>
        public static Font AddFontFile(string path)
        {
            ThrowIfNotInitialized();

            lock (fontInfoLookup)
            {
                FontInfo[] fontInfos = AddFontFileToCache(fontInfoLookup, fontInfoLookupFullName, path);

                if (fontInfos.Length == 0)
                {
                    throw new ArgumentException("File is not an OpenType font or collection", nameof(path));
                }

                return GetFontFromCacheOrLoad(fontInfos[0]);
            }
        }

        /// <summary>
        /// Attempts to get a font given a family name, and optionally a subfamily name.
        /// </summary>
        /// <remarks>
        /// When <paramref name="subfamily"/> is <see langword="null"/>, <paramref name="fallbackIfNoSubfamily"/> is ignored,
        /// and always treated as if it were <see langword="true"/>.
        /// </remarks>
        /// <param name="family">The name of the font family to look for.</param>
        /// <param name="font">The font with that family name, if any.</param>
        /// <param name="subfamily">The font subfamily name.</param>
        /// <param name="fallbackIfNoSubfamily">Whether or not to fallback to the first font with the given family name if the given subfamily name was not found.</param>
        /// <returns><see langword="true"/> if the font was found, <see langword="false"/> otherwise.</returns>
        public static bool TryGetFontByFamily(string family, out Font font, string subfamily = null, bool fallbackIfNoSubfamily = false)
        {
            if (TryGetFontInfoByFamily(family, out FontInfo info, subfamily, fallbackIfNoSubfamily))
            {
                font = GetFontFromCacheOrLoad(info);
                return true;
            }

            font = null;
            return false;
        }

        /// <summary>
        /// Attempts to get a font by its full name.
        /// </summary>
        /// <param name="fullName">The full name of the font to look for.</param>
        /// <param name="font">The font identified by <paramref name="fullName"/>, if any.</param>
        /// <returns><see langword="true"/> if the font was found, <see langword="false"/> otherwise.</returns>
        public static bool TryGetFontByFullName(string fullName, out Font font)
        {
            if (TryGetFontInfoByFullName(fullName, out FontInfo info))
            {
                font = GetFontFromCacheOrLoad(info);
                return true;
            }
            else
            {
                font = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the font fallback list provided by the OS for a given font name, if there is any.
        /// </summary>
        /// <remarks>
        /// If the OS specifies no fallbacks, then the result of this function will be empty.
        /// </remarks>
        /// <param name="fullname">The full name of the font to look up the fallbacks for.</param>
        /// <returns>A list of fallbacks defined by the OS.</returns>
        public static IEnumerable<string> GetOSFontFallbackList(string fullname)
        {
            using RegistryKey syslinkKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontLink\SystemLink");
            if (syslinkKey == null)
            {
                return [];
            }

            object keyVal = syslinkKey.GetValue(fullname);
            if (keyVal is string[] names)
            {
                // the format in this is '<filename>,<font full name>[,<some other stuff>]'
                return names.Select(s => s.Split(',').ElementAtOrDefault(1)?.Trim()).Where(s => !string.IsNullOrEmpty(s));
            }

            return [];
        }

        /// <summary>
        /// Attempts to get a <see cref="TMP_FontAsset"/> with the given family name, and optionally subfamily.
        /// </summary>
        /// <param name="family">The name of the font family to look for.</param>
        /// <param name="font">The font with that family name, if any.</param>
        /// <param name="subfamily">The font subfamily name.</param>
        /// <param name="fallbackIfNoSubfamily">Whether or not to fallback to the first font with the given family name if the given subfamily name was not found.</param>
        /// <param name="setupOsFallbacks">Whether or not to set up the fallbacks specified by the OS.</param>
        /// <returns><see langword="true"/> if the font was found, <see langword="false"/> otherwise.</returns>
        public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font, string subfamily = null, bool fallbackIfNoSubfamily = false, bool setupOsFallbacks = true)
        {
            if (!TryGetFontInfoByFamily(family, out FontInfo info, subfamily, fallbackIfNoSubfamily))
            {
                font = null;
                return false;
            }

            font = GetOrSetupTMPFontFor(info, setupOsFallbacks);
            return true;
        }

        /// <summary>
        /// Attempts to get a <see cref="TMP_FontAsset"/> by its font's full name.
        /// </summary>
        /// <param name="fullName">The full name of the font to look for.</param>
        /// <param name="font">The font identified by <paramref name="fullName"/>, if any.</param>
        /// <param name="setupOsFallbacks">Whether or not to set up the fallbacks specified by the OS.</param>
        /// <returns><see langword="true"/> if the font was found, <see langword="false"/> otherwise.</returns>
        public static bool TryGetTMPFontByFullName(string fullName, out TMP_FontAsset font, bool setupOsFallbacks = true)
        {
            if (!TryGetFontInfoByFullName(fullName, out FontInfo info))
            {
                font = null;
                return false;
            }

            font = GetOrSetupTMPFontFor(info, setupOsFallbacks);
            return true;
        }

        /// <summary>
        /// Create <see cref="TMP_FontAsset"/>s for each font passed in <paramref name="fontNames"/> and their system fallbacks.
        /// </summary>
        /// <param name="fontNames">The font names.</param>
        /// <returns>A list of <see cref="TMP_FontAsset"/>s containing all the fonts specified by <paramref name="fontNames"/> and their system fallbacks.</returns>
        internal static List<TMP_FontAsset> CreateFallbackFonts(TMPFontCreationArgs[] fontNames)
        {
            List<TMP_FontAsset> fontAssets = new(fontNames.Length);

            foreach (TMPFontCreationArgs config in fontNames)
            {
                if (!TryGetFontInfoByFullName(config.FullName, out FontInfo fontInfo))
                {
                    Logger.Log.Warn($"Could not find font '{config.FullName}'; some Unicode characters may not display properly");
                    continue;
                }

                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(GetFontFromCacheOrLoad(fontInfo), config.SamplingPointSize, config.AtlasPadding, config.RenderMode, config.AtlasWidth, config.AtlasHeight);
                fontAsset.name = fontInfo.FullName;
                FaceInfo faceInfo = fontAsset.faceInfo;
                faceInfo.scale = config.Scale;
                fontAsset.faceInfo = faceInfo;

                fontAssets.Add(fontAsset);
            }

            return fontAssets;
        }

        private static void ThrowIfNotInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("FontManager not initialized");
            }
        }

        private static async Task<(Dictionary<string, List<FontInfo>> Families, Dictionary<string, FontInfo> Fulls)> LoadSystemFonts()
        {
            // This should be on the main thread.
            string[] paths = await UnityMainThreadTaskScheduler.Factory.StartNew(Font.GetPathsToOSFonts);

            Dictionary<string, List<FontInfo>> families = new(paths.Length, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, FontInfo> fullNames = new(paths.Length, StringComparer.OrdinalIgnoreCase);

            foreach (string path in paths)
            {
                try
                {
                    AddFontFileToCache(families, fullNames, path);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error($"Failed to load system fonts\n{ex}");
                }

                await Task.Yield();
            }

            return (families, fullNames);
        }

        private static FontInfo[] AddFontFileToCache(Dictionary<string, List<FontInfo>> cache, Dictionary<string, FontInfo> fullCache, string path)
        {
            FontInfo AddFont(OpenTypeFont font)
            {
#if DEBUG
                Logger.Log.Debug($"'{path}' = '{font.Family}' '{font.Subfamily}' ({font.FullName})");
#endif
                FontInfo fontInfo = new(path, font.FullName, font.Subfamily);

                List<FontInfo> list = GetListForFamily(cache, font.Family);
                list.Add(fontInfo);
                fullCache.TryAdd(font.FullName, fontInfo);

                return fontInfo;
            }

            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            using OpenTypeReader reader = OpenTypeReader.For(fileStream);

            switch (reader)
            {
                case OpenTypeCollectionReader colReader:
                    using (OpenTypeCollection collection = new(colReader, lazyLoad: false))
                    {
                        IReadOnlyList<OpenTypeFont> fonts = collection.Fonts;
                        FontInfo[] fontInfos = new FontInfo[fonts.Count];

                        for (int i = 0; i < fonts.Count; i++)
                        {
                            fontInfos[i] = AddFont(fonts[i]);
                        }

                        return fontInfos;
                    }

                case OpenTypeFontReader fontReader:
                    using (OpenTypeFont font = new(fontReader, lazyLoad: false))
                    {
                        return [AddFont(font)];
                    }

                default:
                    Logger.Log.Debug($"Font file '{path}' is not an OpenType file");
                    return [];
            }
        }

        private static List<FontInfo> GetListForFamily(Dictionary<string, List<FontInfo>> cache, string family)
        {
            if (!cache.TryGetValue(family, out List<FontInfo> list))
            {
                cache.Add(family, list = new List<FontInfo>());
            }

            return list;
        }

        private static bool TryGetFontInfoByFamily(string family, out FontInfo info, string subfamily = null, bool fallbackIfNoSubfamily = false)
        {
            ThrowIfNotInitialized();

            if (subfamily == null)
            {
                fallbackIfNoSubfamily = true;
            }

            subfamily ??= "Regular"; // this is a typical default family name

            lock (fontInfoLookup)
            {
                if (fontInfoLookup.TryGetValue(family, out List<FontInfo> fonts))
                {
                    info = fonts.Where(p => p.Subfamily.Equals(subfamily, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (info == null)
                    {
                        if (!fallbackIfNoSubfamily)
                        {
                            return false;
                        }
                        else
                        {
                            info = fonts.First();
                        }
                    }

                    return true;
                }
                else
                {
                    info = null;
                    return false;
                }
            }
        }

        private static bool TryGetFontInfoByFullName(string fullname, out FontInfo info)
        {
            ThrowIfNotInitialized();

            lock (fontInfoLookup)
            {
                return fontInfoLookupFullName.TryGetValue(fullname, out info);
            }
        }

        private static Font GetFontFromCacheOrLoad(FontInfo info)
        {
            lock (LoadedFontsCache)
            {
                if (!LoadedFontsCache.TryGetValue(info.Path, out Font font) || font == null)
                {
                    font = new Font(info.Path)
                    {
                        name = info.FullName,
                    };

                    LoadedFontsCache[info.Path] = font;
                }

                return font;
            }
        }

        private static TMP_FontAsset GetOrSetupTMPFontFor(FontInfo info, bool setupOsFallbacks)
        {
            // don't lock on this because this is mutually recursive with TryGetTMPFontByFullName
            Font font = GetFontFromCacheOrLoad(info);
            TMP_FontAsset tmpFont = BeatSaberUI.CreateTMPFont(font, info.FullName);

            if (setupOsFallbacks)
            {
                Logger.Log.Debug($"Reading fallbacks for '{info.FullName}'");
                foreach (string fallback in GetOSFontFallbackList(info.FullName))
                {
                    Logger.Log.Debug($"Reading fallback '{fallback}'");
                    if (TryGetTMPFontByFullName(fallback, out TMP_FontAsset fallbackFont, false))
                    {
                        tmpFont.fallbackFontAssetTable ??= new List<TMP_FontAsset>();

                        // these fallbacks are used for non-latin characters which are more often than not much larger than Teko
                        FaceInfo faceInfo = fallbackFont.faceInfo;
                        faceInfo.scale = 0.85f;
                        fallbackFont.faceInfo = faceInfo;

                        tmpFont.fallbackFontAssetTable.Add(fallbackFont);
                    }
                    else
                    {
                        Logger.Log.Debug($"-> Not found");
                    }
                }
            }

            return tmpFont;
        }

        internal record TMPFontCreationArgs(string FullName, int SamplingPointSize = 90, int AtlasPadding = 9, GlyphRenderMode RenderMode = GlyphRenderMode.SDFAA, int AtlasWidth = 1024, int AtlasHeight = 1024, float Scale = 0.85f);

        private record FontInfo(string Path, string FullName, string Subfamily);
    }
}
