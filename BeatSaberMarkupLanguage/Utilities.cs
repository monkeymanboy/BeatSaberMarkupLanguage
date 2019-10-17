using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
                    _editIcon = Resources.FindObjectsOfTypeAll<Image>().First(x => x.sprite?.name == "EditIcon").sprite;

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
    }
}
