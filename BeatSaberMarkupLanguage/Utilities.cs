﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace BeatSaberMarkupLanguage
{
    public static class Utilities
    {
        private static Sprite _editIcon = null;

        public static Sprite EditIcon
        {
            get
            {
                if (!_editIcon)
                    _editIcon = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>().First(x => x.sprite?.name == "EditIcon").sprite;

                return _editIcon;
            }
        }

        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static List<T> GetListOfType<T>(params object[] constructorArgs)
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));

            return objects;
        }

        //yoinked from https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType())
                return null; // type mismatch

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
                finfo.SetValue(comp, finfo.GetValue(other));

            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

        //end of yoink

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
            private static Sprite _blankSprite = null;

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
                    if (!_blankSprite)
                        _blankSprite = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);

                    return _blankSprite;
                }
            }
        }

        public static Texture2D FindTextureInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                    return LoadTextureRaw(GetResource(asm, newPath));
            }
            catch (Exception ex)
            {
                Logger.log?.Error("Unable to find texture in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
            }
            return null;
        }

        public static Sprite FindSpriteInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                    return LoadSpriteRaw(GetResource(asm, newPath));
            }
            catch (Exception ex)
            {
                Logger.log?.Error("Unable to find sprite in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
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
                Texture2D Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(file))
                    return Tex2D;
            }
            return null;
        }

        public static Sprite LoadSpriteRaw(byte[] image, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (SpriteTexture)
                return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
            return null;
        }

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        /// <summary>
        /// Get data from either a resource path, a file path, or a url
        /// </summary>
        /// <param name="location">Resource path, file path, or url. May need to prefix resource paths with 'AssemblyName:'</param>
        /// <param name="callback">Received data</param>
        public static void GetData(string location, Action<byte[]> callback)
        {
            try
            {
                if (location.StartsWith("http://") || location.StartsWith("https://"))
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
                Logger.log.Error($"Error getting data from '{location}' either invalid path or file does not exist");
            }
        }

        private static IEnumerator GetWebDataCoroutine(string url, Action<byte[]> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Logger.log.Debug($"Error getting data from {url}, Message:{www.error}");
            }
            else
            {
                callback?.Invoke(www.downloadHandler.data);
            }
        }
    }
}
