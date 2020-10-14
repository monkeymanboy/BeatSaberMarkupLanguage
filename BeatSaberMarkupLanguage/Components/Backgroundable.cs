using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Backgroundable : MonoBehaviour
    {
        private static Dictionary<string, string> Backgrounds => new Dictionary<string, string>()
        {
            { "round-rect-panel", "RoundRect10" },
            { "panel-bottom", "PanelBottom" },
            { "panel-top", "RoundRect10" },
            { "round-rect-panel-shadow", "RoundRectPanelShadow"}
        };

        private static Dictionary<string, string> ObjectNames => new Dictionary<string, string>()
        {
            { "round-rect-panel", "KeyboardWrapper" },
            { "panel-bottom", "BG" },
            { "panel-top", "BG" },
            { "round-rect-panel-shadow", "Shadow"}
        };

        public Image background;

        public void ApplyBackground(string name)
        {
            /*
            foreach(Image image in Resources.FindObjectsOfTypeAll<Image>())
            {
                Console.WriteLine($"{image.gameObject.name} - {image.sprite?.name}");
            }
            */

            if (background != null)
                throw new Exception("Cannot add multiple backgrounds");

            if (!Backgrounds.TryGetValue(name, out string backgroundName))
                throw new Exception($"Background type '{name}' not found");

            try { 
                background = gameObject.AddComponent(Resources.FindObjectsOfTypeAll<ImageView>().First(x => x.gameObject?.name == ObjectNames[name] && x.sprite?.name == backgroundName));
                background.enabled = true;
            }
            catch
            {
                Logger.log.Error($"Error loading background: '{name}'");
            }
        }
    }
}
