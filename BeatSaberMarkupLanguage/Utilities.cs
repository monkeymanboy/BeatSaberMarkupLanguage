using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage
{
    public static class Utilities
    {
        private static Sprite editIcon = null;

        public static Sprite EditIcon
        {
            get
            {
                if (editIcon == null)
                {
                    editIcon = Resources.FindObjectsOfTypeAll<Image>().Where(x => x.sprite != null).First(x => x.sprite.name == "EditIcon").sprite;
                }

                return editIcon;
            }
        }

        /// <summary>
        /// Gets the content of a resource as a string.
        /// </summary>
        /// <param name="assembly">Assembly containing the resource.</param>
        /// <param name="resource">Full path to the resource.</param>
        /// <returns>The contents of the resource as a string.</returns>
        /// <exception cref="BSMLResourceException">Thrown if any exception occurs while loading the resource.</exception>
        public static string GetResourceContent(Assembly assembly, string resource)
        {
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new BSMLResourceException(assembly, resource, ex);
            }
        }

        public static List<T> GetListOfType<T>(params object[] constructorArgs)
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }

            return objects;
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

        public static string EscapeXml(string source)
        {
            return source.Replace("\"", "&quot;")
                .Replace("\"", "&quot;")
                .Replace("&", "&amp;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
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
                    if (noGlowMat == null)
                    {
                        noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First());
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
                        whitePixel = Resources.FindObjectsOfTypeAll<Image>().Where(i => i.sprite != null).First(i => i.sprite.name == "WhitePixel").sprite;
                    }

                    return whitePixel;
                }
            }
        }

        public static Texture2D FindTextureInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                {
                    return LoadTextureRaw(GetResource(asm, newPath));
                }
            }
            catch (Exception ex)
            {
                Logger.Log?.Error("Unable to find texture in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
            }

            return null;
        }

        public static Sprite FindSpriteInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                {
                    return LoadSpriteRaw(GetResource(asm, newPath));
                }
            }
            catch (Exception ex)
            {
                Logger.Log?.Error("Unable to find sprite in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
            }

            return null;
        }

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
                    throw new Exception($"Could not process resource path {inputPath}");
            }
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
                if (tex2D.LoadImage(file))
                {
                    return tex2D;
                }
            }

            return null;
        }

        public static Sprite LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), pixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            if (spriteTexture == null)
            {
                return null;
            }

            return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
        }

        public static byte[] GetResource(Assembly asm, string resourceName)
        {
            Stream stream = asm.GetManifestResourceStream(resourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        public static IEnumerable<T> SingleEnumerable<T>(this T item)
            => Enumerable.Empty<T>().Append(item);

        public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> seq)
            where T : struct
            => seq.Select(v => new T?(v));

        public static T? AsNullable<T>(this T item)
            where T : struct
            => item;

        /// <summary>
        /// Get data from either a resource path, a file path, or a url.
        /// </summary>
        /// <param name="location">Resource path, file path, or url. May need to prefix resource paths with 'AssemblyName:'.</param>
        /// <param name="callback">Received data.</param>
        public static void GetData(string location, Action<byte[]> callback)
        {
            try
            {
                if (location.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || location.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    SharedCoroutineStarter.instance.StartCoroutine(GetWebDataCoroutine(location, callback));
                }
                else if (File.Exists(location))
                {
                    callback?.Invoke(File.ReadAllBytes(location));
                }
                else
                {
                    AssemblyFromPath(location, out Assembly asm, out string newPath);
                    callback?.Invoke(GetResource(asm, newPath));
                }
            }
            catch
            {
                Logger.Log.Error($"Error getting data from '{location}' either invalid path or file does not exist");
            }
        }

        private static IEnumerator GetWebDataCoroutine(string url, Action<byte[]> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Logger.Log.Debug($"Error getting data from {url}, Message:{www.error}");
            }
            else
            {
                callback?.Invoke(www.downloadHandler.data);
            }
        }

        public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        internal static Sprite FindSpriteCached(string name)
        {
            if (spriteCache.TryGetValue(name, out var sprite) && sprite != null)
            {
                return sprite;
            }

            foreach (var x in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (x.name.Length == 0)
                {
                    continue;
                }

                if (!spriteCache.TryGetValue(x.name, out var a) || a == null)
                {
                    spriteCache[x.name] = x;
                }

                if (x.name == name)
                {
                    sprite = x;
                }
            }

            return sprite;
        }

        public static Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        internal static Texture FindTextureCached(string name)
        {
            if (textureCache.TryGetValue(name, out var texture) && texture != null)
            {
                return texture;
            }

            foreach (var x in Resources.FindObjectsOfTypeAll<Texture>())
            {
                if (x.name.Length == 0)
                {
                    continue;
                }

                if (!textureCache.TryGetValue(x.name, out var a) || a == null)
                {
                    textureCache[x.name] = x;
                }

                if (x.name == name)
                {
                    texture = x;
                }
            }

            return texture;
        }
    }
}
