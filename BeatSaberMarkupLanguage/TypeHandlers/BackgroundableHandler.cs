using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler
    {
        private Dictionary<string, string> Backgrounds => new Dictionary<string, string>()
        {
            { "round-rect-panel", "RoundRectPanel" },
            { "panel-bottom", "PanelBottom" },
            { "panel-top", "PanelTop" }
        };
        private Dictionary<string, string> ObjectNames => new Dictionary<string, string>()
        {
            { "round-rect-panel", "MinScoreInfo" },
            { "panel-bottom", "BG" },
            { "panel-top", "HeaderPanel" }
        };
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "background", new[]{ "bg", "background" } },
            { "backgroundColor", new[]{ "bg-color", "background-color" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, BSMLAction> actions)
        {
            if (!data.ContainsKey("background")) return;
            if (!Backgrounds.ContainsKey(data["background"]))
                throw new Exception("Background type '" + data["background"] + "' not found");
            Image image = obj.gameObject.AddComponent(Resources.FindObjectsOfTypeAll<Image>().Last(x => x.gameObject.name == ObjectNames[data["background"]] && x.sprite?.name == Backgrounds[data["background"]]));
            if (data.ContainsKey("backgroundColor") && data["backgroundColor"] != "none")
            {
                ColorUtility.TryParseHtmlString(data["backgroundColor"], out Color color);
                image.color = color;
            }
        }
    }
}
