using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Bitmap = System.Drawing.Bitmap;

namespace BeatSaberMarkupLanguage
{
    public static class Utilities
    {
        // this is technically wrong because TMP will only parse tags it recognizes but whatever
        private static readonly Regex HtmlTagsRegex = new(@"<([a-z\-]+(=""?([a-z]+|[\+\-]?[0-9\.]+[%pe]?)""?)?)( ([a-z\-]+(=""?([a-z]+|[\+\-]?[0-9\.]+[%pe]?)""?)?))*>", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private static Sprite editIcon = null;

        public static Sprite EditIcon
        {
            get
            {
                if (editIcon == null)
                {
                    editIcon = Resources.FindObjectsOfTypeAll<Image>().Where(x => x.sprite != null && x.sprite.name == "EditIcon").First().sprite;
                }

                return editIcon;
            }
        }

        public static Dictionary<string, Sprite> SpriteCache { get; } = new();

        public static Dictionary<string, Texture> TextureCache { get; } = new();

        /// <summary>
        /// Gets the content of a resource as a string.
        /// </summary>
        /// <param name="assembly">Assembly containing the resource.</param>
        /// <param name="resource">Full path to the resource.</param>
        /// <returns>The contents of the resource as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the resource specified by <paramref name="resource"/> cannot be found in <paramref name="assembly"/>.</exception>
        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using Stream stream = assembly.GetManifestResourceStream(resource) ?? throw new ResourceNotFoundException(assembly, resource);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        // yoinked from https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        public static T GetCopyOf<T>(this Component comp, T other)
            where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType())
            {
                return null; // type mismatch
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (PropertyInfo pinfo in pinfos)
            {
                if (pinfo.CanWrite && pinfo.Name != "name")
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                    }
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);
            foreach (FieldInfo finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd)
            where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd);
        }

        public static string EscapeXml(string source) => System.Security.SecurityElement.Escape(source);

        public static void AssemblyFromPath(string inputPath, out Assembly assembly, out string path)
        {
            string[] parameters = inputPath.Split(':');
            switch (parameters.Length)
            {
                case 1:
                    path = parameters[0];
                    assembly = Assembly.Load(path.Substring(0, path.IndexOf('.')));
                    break;
                case 2:
                    path = parameters[1];
                    assembly = Assembly.Load(parameters[0]);
                    break;
                default:
                    throw new BSMLException($"Could not process resource path {inputPath}");
            }
        }

        /// <summary>
        /// Load a texture from an embedded resource in the calling assembly.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static Task<Texture2D> LoadTextureFromAssemblyAsync(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return LoadTextureFromAssemblyAsync(assembly, name);
        }

        /// <summary>
        /// Load a texture from an embedded resource in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to load the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadTextureFromAssemblyAsync(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);

            return stream != null
                ? await LoadImageAsync(stream)
                : throw new FileNotFoundException($"No embedded resource named '{name}' found in assembly '{assembly.FullName}'");
        }

        /// <summary>
        /// Load a sprite from an embedded resource in the calling assembly.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static Task<Sprite> LoadSpriteFromAssemblyAsync(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return LoadSpriteFromAssemblyAsync(assembly, name);
        }

        /// <summary>
        /// Load a sprite from an embedded resource in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to load the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Sprite> LoadSpriteFromAssemblyAsync(Assembly assembly, string name)
        {
            return LoadSpriteFromTexture(await LoadTextureFromAssemblyAsync(assembly, name));
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(string path, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            using FileStream fileStream = File.OpenRead(path);
            return await LoadImageAsync(fileStream, updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="data">The image data as a byte array.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(byte[] data, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using MemoryStream memoryStream = new(data);
            return await LoadImageAsync(memoryStream, updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="stream">The image data as a stream.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(Stream stream, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            EnsureRunningOnMainThread();

            (int width, int height, byte[] data) = await Task.Factory.StartNew(
                () =>
                {
                    using Bitmap bitmap = new(stream);

                    // flip it over since Unity uses OpenGL coordinates - (0, 0) is the bottom left corner instead of the top left
                    bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

                    BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    byte[] data = new byte[bitmapData.Stride * bitmapData.Height];

                    Marshal.Copy(bitmapData.Scan0, data, 0, bitmapData.Stride * bitmapData.Height);

                    bitmap.UnlockBits(bitmapData);

                    return (bitmap.Width, bitmap.Height, data);
                },
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

            // basically all processors are little endian these days so pixel format order is reversed
            Texture2D texture = new(width, height, TextureFormat.BGRA32, false);
            texture.LoadRawTextureData(data);
            texture.Apply(updateMipmaps, makeNoLongerReadable);
            return texture;
        }

        public static async Task<Sprite> LoadSpriteAsync(byte[] data, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(await LoadImageAsync(data), pixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            if (spriteTexture == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
            sprite.name = spriteTexture.name;
            return sprite;
        }

        [Obsolete("Use GetResourceAsync instead")]
        public static byte[] GetResource(Assembly asm, string resourceName)
        {
            using Stream resourceStream = asm.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new(new byte[resourceStream.Length], true);

            resourceStream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        public static async Task<byte[]> GetResourceAsync(Assembly asm, string resourceName)
        {
            using Stream resourceStream = asm.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new(new byte[resourceStream.Length], true);

            await resourceStream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public static IEnumerable<T> SingleEnumerable<T>(this T item)
            => Enumerable.Empty<T>().Append(item);

        public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> seq)
            where T : struct
            => seq.Select(v => new T?(v));

        public static T? AsNullable<T>(this T item)
            where T : struct
            => item;

        internal static void EnsureRunningOnMainThread()
        {
            if (!UnityGame.OnMainThread)
            {
                throw new InvalidOperationException("This method can only be called from the main thread.");
            }
        }

        internal static async Task<byte[]> GetDataAsync(string location)
        {
            if (location.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || location.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return await GetWebDataAsync(location);
            }
            else if (File.Exists(location))
            {
                using (FileStream fileStream = File.OpenRead(location))
                using (MemoryStream memoryStream = new(new byte[fileStream.Length], true))
                {
                    await fileStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
            {
                AssemblyFromPath(location, out Assembly asm, out string newPath);
                return await GetResourceAsync(asm, newPath);
            }
        }

        internal static IEnumerable<Type> GetDescendants<T>(params object[] constructorArgs)
        {
            return Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
        }

        internal static IEnumerable<T> GetInstancesOfDescendants<T>(params object[] constructorArgs)
        {
            foreach (Type type in GetDescendants<T>())
            {
                yield return (T)Activator.CreateInstance(type, constructorArgs);
            }
        }

        internal static Sprite FindSpriteCached(string name)
        {
            if (SpriteCache.TryGetValue(name, out Sprite sprite) && sprite != null)
            {
                return sprite;
            }

            foreach (Sprite x in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (x.name.Length == 0)
                {
                    continue;
                }

                if (!SpriteCache.TryGetValue(x.name, out Sprite a) || a == null)
                {
                    SpriteCache[x.name] = x;
                }

                if (x.name == name)
                {
                    sprite = x;
                }
            }

            return sprite;
        }

        internal static Texture FindTextureCached(string name)
        {
            if (TextureCache.TryGetValue(name, out Texture texture) && texture != null)
            {
                return texture;
            }

            foreach (Texture x in Resources.FindObjectsOfTypeAll<Texture>())
            {
                if (x.name.Length == 0)
                {
                    continue;
                }

                if (!TextureCache.TryGetValue(x.name, out Texture a) || a == null)
                {
                    TextureCache[x.name] = x;
                }

                if (x.name == name)
                {
                    texture = x;
                }
            }

            return texture;
        }

        internal static string StripHtmlTags(string str)
        {
            return HtmlTagsRegex.Replace(str, string.Empty);
        }

        private static Task<byte[]> GetWebDataAsync(string url)
        {
            TaskCompletionSource<byte[]> taskCompletionSource = new();
            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            webRequest.SendWebRequest().completed += (asyncOperation) =>
            {
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    taskCompletionSource.SetException(new UnityWebRequestException("Failed to get data", webRequest));
                }
                else
                {
                    taskCompletionSource.SetResult(webRequest.downloadHandler.data);
                }
            };

            return taskCompletionSource.Task;
        }

        public static class ImageResources
        {
            private static Material noGlowMat;
            private static Sprite blankSprite;
            private static Sprite whitePixel;

            public static Material NoGlowMat
            {
                get
                {
                    if (noGlowMat == null && BeatSaberUI.TryGetSoloButton(out Button soloButton))
                    {
                        noGlowMat = new Material(soloButton.transform.Find("Image/Image0").GetComponent<Image>().material);
                        noGlowMat.name = "UINoGlowCustom";
                    }

                    return noGlowMat;
                }
            }

            public static Sprite BlankSprite
            {
                get
                {
                    if (!blankSprite)
                    {
                        blankSprite = Sprite.Create(Texture2D.blackTexture, default, Vector2.zero);
                    }

                    return blankSprite;
                }
            }

            public static Sprite WhitePixel
            {
                get
                {
                    if (!whitePixel)
                    {
                        whitePixel = Resources.FindObjectsOfTypeAll<Image>().Where(i => i.sprite != null && i.sprite.name == "WhitePixel").First().sprite;
                    }

                    return whitePixel;
                }
            }
        }
    }
}
