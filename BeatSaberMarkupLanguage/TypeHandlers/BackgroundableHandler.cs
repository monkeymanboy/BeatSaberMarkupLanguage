using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "background", new[]{ "bg", "background" } },
            { "backgroundColor", new[]{ "bg-color", "background-color" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            Backgroundable backgroundable = obj as Backgroundable;
            if (data.ContainsKey("background"))
            {
                backgroundable.ApplyBackground(data["background"]);
            }

            if (data.ContainsKey("backgroundColor") && data["backgroundColor"] != "none")
            {
                ColorUtility.TryParseHtmlString(data["backgroundColor"], out Color color);
                backgroundable.background.color = color;
            }
        }
    }
}
