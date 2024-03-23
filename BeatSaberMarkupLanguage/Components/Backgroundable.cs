using System.Collections.Generic;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Backgroundable : MonoBehaviour
    {
        public Image background;

        private static readonly Dictionary<string, ImageView> BackgroundCache = new();

        private static Dictionary<string, string> Backgrounds => new()
        {
            { "round-rect-panel", "RoundRect10" },
            { "panel-top", "RoundRect10" },
            { "panel-fade-gradient", "RoundRect10Thin" },
            { "panel-top-gradient", "RoundRect10" },
        };

        private static Dictionary<string, string> ObjectNames => new()
        {
            { "round-rect-panel", "KeyboardWrapper" },
            { "panel-top", "BG" },
            { "panel-fade-gradient", "Background" },
            { "panel-top-gradient", "BG" },
        };

        private static Dictionary<string, string> ObjectParentNames => new()
        {
            { "round-rect-panel", "Wrapper" },
            { "panel-top", "PracticeButton" },
            { "panel-fade-gradient", "LevelListTableCell" },
            { "panel-top-gradient", "ActionButton" },
        };

        public void ApplyBackground(string name)
        {
            if (background != null)
            {
                throw new BSMLException("Cannot add multiple backgrounds");
            }

            if (!Backgrounds.TryGetValue(name, out string backgroundName))
            {
                throw new BSMLException($"Background type '{name}' not found");
            }

            try
            {
                if (!BackgroundCache.TryGetValue(name, out ImageView bgTemplate) || bgTemplate == null)
                {
                    if (!bgTemplate)
                    {
                        BackgroundCache.Remove(name);
                    }

                    bgTemplate = FindTemplate(name, backgroundName);
                    BackgroundCache.Add(name, bgTemplate);
                }

                background = gameObject.AddComponent(bgTemplate);
                background.enabled = true;
            }
            catch
            {
                Logger.Log.Error($"Error loading background: '{name}'");
            }
        }

        private static ImageView FindTemplate(string name, string backgroundName)
        {
            string objectName = ObjectNames[name];
            string parentName = ObjectParentNames[name];
            ImageView[] images = FindObjectsOfType<ImageView>(true);
            for (int i = 0; i < images.Length; i++)
            {
                ImageView image = images[i];
                Sprite sprite = image.sprite;
                if (!sprite || sprite.name != backgroundName)
                {
                    continue;
                }

                Transform parent = image.transform.parent;
                if (!parent || parent.name != parentName)
                {
                    continue;
                }

                if (image.gameObject == null || image.gameObject.name != objectName)
                {
                    continue;
                }

                return image;
            }

            return null;
        }
    }
}
