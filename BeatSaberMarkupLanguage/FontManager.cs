using BeatSaberMarkupLanguage.OpenType;
using IPA.Utilities.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage
{
    public static class FontManager
    {
        private class FontInfo
        {
            public string Path;
            public OpenTypeFont Info;
            public FontInfo(string path, OpenTypeFont info)
            {
                Path = path;
                Info = info;
            }
        }

        // family -> list of fonts in family
        private static Dictionary<string, List<FontInfo>> fontInfoLookup;
        // full name -> font info
        private static Dictionary<string, FontInfo> fontInfoLookupFullName;
        // path -> loaded font object
        private static readonly Dictionary<string, Font> loadedFontsCache = new Dictionary<string, Font>();
        // (unity font, has system fallback set) -> tmp font
        private static readonly Dictionary<(Font font, bool hasFallbacks), TMP_FontAsset> tmpFontCache = new Dictionary<(Font font, bool hasFallbacks), TMP_FontAsset>();

        public static Task SystemFontLoadTask { get; private set; }

        public static Task AsyncLoadSystemFonts()
        {
            if (IsInitialized) return Task.CompletedTask;
            if (SystemFontLoadTask != null) return SystemFontLoadTask;
            var task = Task.Factory.StartNew(LoadSystemFonts).Unwrap();
            SystemFontLoadTask = task.ContinueWith(t =>
            {
                Logger.log.Debug("Font loading complete");
                Interlocked.CompareExchange(ref fontInfoLookupFullName, t.Result.fulls, null);
                Interlocked.CompareExchange(ref fontInfoLookup, t.Result.families, null);
                return Task.CompletedTask;
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();
            task.ContinueWith(t => Logger.log.Error($"Font loading errored: {t.Exception}"), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion);
            return task;
        }

        public static Task Destroy()
        {
            fontInfoLookup = null;
            fontInfoLookupFullName = null;
            return UnityMainThreadTaskScheduler.Factory.StartNew(() => DestroyObjects(loadedFontsCache.Select(p => p.Value))).Unwrap()
                .ContinueWith(_ => loadedFontsCache.Clear())
                .ContinueWith(_ => DestroyObjects(tmpFontCache.Select(p => p.Value)), UnityMainThreadTaskScheduler.Default).Unwrap()
                .ContinueWith(_ => tmpFontCache.Clear());
        }

        private static async Task DestroyObjects(IEnumerable<UnityEngine.Object> objects)
        {
            foreach (var obj in objects)
            {
                UnityEngine.Object.Destroy(obj);
                await Task.Yield(); // yield back to the scheduler to allow more things to happen
            }
        }

        private static async Task<(Dictionary<string, List<FontInfo>> families, Dictionary<string, FontInfo> fulls)> LoadSystemFonts()
        {
            var paths = Font.GetPathsToOSFonts();

            var families = new Dictionary<string, List<FontInfo>>(paths.Length, StringComparer.InvariantCultureIgnoreCase);
            var fullNames = new Dictionary<string, FontInfo>(paths.Length, StringComparer.InvariantCultureIgnoreCase);

            foreach (var path in paths)
            {
                try
                {
                    AddFontFileToCache(families, fullNames, path);
                }
                catch (Exception e)
                {
                    Logger.log.Error(e);
                }

                await Task.Yield();
            }

            return (families, fullNames);
        }

        private static IEnumerable<FontInfo> AddFontFileToCache(Dictionary<string, List<FontInfo>> cache, Dictionary<string, FontInfo> fullCache, string path)
        {
            FontInfo AddFont(OpenTypeFont font)
            {
#if DEBUG
                Logger.log.Debug($"'{path}' = '{font.Family}' '{font.Subfamily}' ({font.FullName})");
#endif
                var fontInfo = new FontInfo(path, font);

                var list = GetListForFamily(cache, font.Family);
                list.Add(fontInfo);
                var name = font.FullName;
                if (fullCache.ContainsKey(name))
                {
                    Logger.log.Warn($"Duplcicate font with full name '{name}' at {path}");
                }
                else
                {
                    fullCache.Add(name, fontInfo);
                }
                return fontInfo;
            }

            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var reader = OpenTypeReader.For(fileStream);
            if (reader is OpenTypeCollectionReader colReader)
            {
                var collection = new OpenTypeCollection(colReader, lazyLoad: false);
                return collection.Select(AddFont).ToList();
            }
            else if (reader is OpenTypeFontReader fontReader)
            {
                var font = new OpenTypeFont(fontReader, lazyLoad: false);
                ;
                return Utilities.SingleEnumerable(AddFont(font));
            }
            else
            {
                Logger.log.Warn($"Font file '{path}' is not an OpenType file");
                return Enumerable.Empty<FontInfo>();
            }
        }

        private static List<FontInfo> GetListForFamily(Dictionary<string, List<FontInfo>> cache, string family)
        {
            if (!cache.TryGetValue(family, out var list))
                cache.Add(family, list = new List<FontInfo>());
            return list;
        }

        public static Font AddFileToCache(string path)
        {
            ThrowIfNotInitialized();

            lock (fontInfoLookup)
            {
                var set = AddFontFileToCache(fontInfoLookup, fontInfoLookupFullName, path);
                if (!set.Any())
                    throw new ArgumentException("File is not an OpenType font or collection", nameof(path));
                return GetFontFromCacheOrLoad(set.First());
            }
        }

        public static bool IsInitialized => fontInfoLookup != null;

        private static void ThrowIfNotInitialized()
        {
            if (!IsInitialized) throw new InvalidOperationException("FontManager not initialized");
        }

        public static bool TryGetFontByFamily(string family, out Font font, string subfamily = null, bool fallbackIfNoSubfamily = false)
        {
            ThrowIfNotInitialized();

            if (subfamily == null) fallbackIfNoSubfamily = true;
            subfamily ??= "Regular"; // this is a typical default family name

            lock (fontInfoLookup)
            {
                if (fontInfoLookup.TryGetValue(family, out var fonts))
                {
                    var info = fonts.FirstOrDefault(p => p?.Info.Subfamily == subfamily);
                    if (info == null)
                    {
                        if (!fallbackIfNoSubfamily)
                        {
                            font = null;
                            return false;
                        }
                        else
                        {
                            info = fonts.First();
                        }
                    }

                    font = GetFontFromCacheOrLoad(info);
                    return true;
                }
                else
                {
                    font = null;
                    return false;
                }
            }
        }

        public static bool TryGetFontByFullName(string fullName, out Font font)
        {
            ThrowIfNotInitialized();

            lock (fontInfoLookup)
            {
                if (fontInfoLookupFullName.TryGetValue(fullName, out var info))
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
        }

        private static Font GetFontFromCacheOrLoad(FontInfo info)
        {
            lock (loadedFontsCache)
            {
                if (!loadedFontsCache.TryGetValue(info.Path, out var font))
                {
                    font = new Font(info.Path);
                    font.name = info.Info.FullName;
                    loadedFontsCache.Add(info.Path, font);
                }
                return font;
            }
        }
    }
}
