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
            { "round-rect-panel", "RoundRectPanel" },
            { "panel-bottom", "PanelBottom" },
            { "panel-top", "PanelTop" }
        };

        private static Dictionary<string, string> ObjectNames => new Dictionary<string, string>()
        {
            { "round-rect-panel", "MinScoreInfo" },
            { "panel-bottom", "BG" },
            { "panel-top", "HeaderPanel" }
        };

        public Image background;

        public void ApplyBackground(string name)
        {
            if (background != null)
                throw new Exception("Cannot add multiple backgrounds");

            if (!Backgrounds.ContainsKey(name))
                throw new Exception("Background type '" + name + "' not found");

            background = gameObject.AddComponent(Resources.FindObjectsOfTypeAll<Image>().Last(x => x.gameObject.name == ObjectNames[name] && x.sprite?.name == Backgrounds[name]));
        }
    }
}
