using BeatSaberMarkupLanguage.OpenType;
using IPA.Utilities.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage
{
    public static class FontManager
    {
        private struct FontInfo
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
        // path -> loaded font object
        private static readonly Dictionary<string, Font> loadedFontsCache = new Dictionary<string, Font>();

        public static Task SystemFontLoadTask { get; private set; }

        public static Task AsyncLoadSystemFonts()
        {
            if (IsInitialized) return Task.CompletedTask;
            if (SystemFontLoadTask != null) return SystemFontLoadTask;
            var task = Task.Factory.StartNew(LoadSystemFonts).Unwrap();
            SystemFontLoadTask = task.ContinueWith(t =>
            {
                Logger.log.Debug("Font loading complete");
                Interlocked.CompareExchange(ref fontInfoLookup, t.Result, null);
                return Task.CompletedTask;
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => Logger.log.Error($"Font loading errored: {t.Exception}"), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion);
            return task;
        }

        public static Task Destroy()
        {
            fontInfoLookup = null;
            return UnityMainThreadTaskScheduler.Factory.StartNew(() => DestroyDict(loadedFontsCache)).Unwrap()
                .ContinueWith(_ => loadedFontsCache.Clear());
        }

        private static async Task DestroyDict(IReadOnlyDictionary<string, Font> dict)
        {
            foreach (var pair in dict)
            {
                Font.Destroy(pair.Value);
                await Task.Yield(); // yield back to the scheduler to allow more things to happen
            }
        }

        private static async Task<Dictionary<string, List<FontInfo>>> LoadSystemFonts()
        {
            var paths = Font.GetPathsToOSFonts();

            var fonts = new Dictionary<string, List<FontInfo>>(paths.Length, StringComparer.InvariantCultureIgnoreCase);

            foreach (var path in paths)
            {
                try
                {
                    AddFontFileToCache(fonts, path);
                }
                catch (Exception e)
                {
                    Logger.log.Error(e);
                }

                await Task.Yield();
            }

            return fonts;
        }

        private static IEnumerable<OpenTypeFont> AddFontFileToCache(Dictionary<string, List<FontInfo>> cache, string path)
        {
            void AddFont(OpenTypeFont font)
            {
#if DEBUG
                Logger.log.Debug($"'{path}' = '{font.Family}' '{font.Subfamily}' ({font.FullName})");
#endif
                var list = GetListForFamily(cache, font.Family);
                list.Add(new FontInfo(path, font));
            }

            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var reader = OpenTypeReader.For(fileStream);
            if (reader is OpenTypeCollectionReader colReader)
            {
                var collection = new OpenTypeCollection(colReader, lazyLoad: false);
                foreach (var font in collection)
                    AddFont(font);
                return collection;
            }
            else if (reader is OpenTypeFontReader fontReader)
            {
                var font = new OpenTypeFont(fontReader, lazyLoad: false);
                AddFont(font);
                return Utilities.SingleEnumerable(font);
            }
            else
            {
                Logger.log.Warn($"Font file '{path}' is not an OpenType file");
                return Enumerable.Empty<OpenTypeFont>();
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
                var set = AddFontFileToCache(fontInfoLookup, path);
                if (!set.Any())
                    throw new ArgumentException("File is not an OpenType font or collection", nameof(path));
                return GetFontFromCacheOrLoad(new FontInfo(path, set.First()));
            }
        }

        public static bool IsInitialized => fontInfoLookup != null;

        private static void ThrowIfNotInitialized()
        {
            if (!IsInitialized) throw new InvalidOperationException("FontManager not initialized");
        }

        public static bool TryGetFont(string family, out Font font, string subfamily = null, bool fallbackIfNoSubfamily = false)
        {
            ThrowIfNotInitialized();

            if (subfamily == null) fallbackIfNoSubfamily = true;
            subfamily ??= "Regular"; // this is a typical default family name

            lock (fontInfoLookup)
            {
                if (fontInfoLookup.TryGetValue(family, out var fonts))
                {
                    var info = fonts.AsNullable().FirstOrDefault(p => p?.Info.Subfamily == subfamily);
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

                    font = GetFontFromCacheOrLoad(info.Value);
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
