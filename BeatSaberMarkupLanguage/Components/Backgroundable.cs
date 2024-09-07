using System.Collections.Generic;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Backgroundable : MonoBehaviour
    {
        // TODO: this should be an ImageView
        public Image Background;

        private static readonly Dictionary<string, ImageView> BackgroundCache = new();

        private static Dictionary<string, string> Backgrounds => new()
        {
            { "round-rect-panel", "RoundRect10" },
            { "panel-top", "RoundRect10" },
            { "panel-fade-gradient", "RoundRect10Thin" },
            { "panel-top-gradient", "RoundRect10" },
            { "title-gradient", "RoundRect10" },
        };

        private static Dictionary<string, string> ObjectNames => new()
        {
            { "round-rect-panel", "KeyboardWrapper" },
            { "panel-top", "BG" },
            { "panel-fade-gradient", "Background" },
            { "panel-top-gradient", "BG" },
            { "title-gradient", "BG" },
        };

        private static Dictionary<string, string> ObjectParentNames => new()
        {
            { "round-rect-panel", "Wrapper" },
            { "panel-top", "PracticeButton" },
            { "panel-fade-gradient", "LevelListTableCell" },
            { "panel-top-gradient", "ActionButton" },
            { "title-gradient", "TitleViewController" },
        };

        public void ApplyBackground(string name)
        {
            if (Background != null)
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

                Background = gameObject.AddComponent(bgTemplate);
                Background.enabled = true;
            }
            catch
            {
                Logger.Log.Error($"Error loading background: '{name}'");
            }
        }

        public void ApplyColor(Color color)
        {
            if (Background == null)
            {
                throw new BSMLException("Can't set color on null background!");
            }

            color.a = Background.color.a;
            Background.color = color;

            if (Background is ImageView imageView)
            {
                Color color0 = new(1, 1, 1, imageView.color0.a);
                Color color1 = new(1, 1, 1, imageView.color1.a);

                imageView.gradient = false;
                imageView.color0 = color0;
                imageView.color1 = color1;
            }
        }

        public void ApplyGradient(Color color0, Color color1)
        {
            if (Background is not ImageView imageView)
            {
                throw new BSMLException("Can't set gradient on null background!");
            }

            Color color = new(1, 1, 1, Background.color.a);

            imageView.gradient = true;
            imageView.color = color;
            imageView.color0 = color0;
            imageView.color1 = color1;
        }

        public void ApplyColor0(Color color0)
        {
            if (Background is not ImageView imageView)
            {
                throw new BSMLException("Can't set gradient on null background!");
            }

            ApplyGradient(color0, imageView.color1);
        }

        public void ApplyColor1(Color color1)
        {
            if (Background is not ImageView imageView)
            {
                throw new BSMLException("Can't set gradient on null background!");
            }

            ApplyGradient(imageView.color0, color1);
        }

        public void ApplyAlpha(float alpha)
        {
            if (Background == null)
            {
                throw new BSMLException("Can't set gradient on null background!");
            }

            Color color = Background.color;
            color.a = alpha;
            Background.color = color;
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
