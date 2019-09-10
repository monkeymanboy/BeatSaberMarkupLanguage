using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LayoutGroup))]
    public class LayoutGroupHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "padTop", new[]{"pad-top"} },
            { "padBottom", new[]{ "pad-bottom" } },
            { "padLeft", new[]{ "pad-left" } },
            { "padRight", new[]{ "pad-right" } },
            { "pad", new[]{ "pad" } },
            { "childAlign", new[] { "child-align" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            LayoutGroup layoutGroup = (obj as LayoutGroup);
            if (data.ContainsKey("pad"))
            {
                int padding = Parse.Int(data["pad"]);
                layoutGroup.padding = new RectOffset(padding, padding, padding, padding);
            }

            layoutGroup.padding = new RectOffset(data.ContainsKey("padLeft") ? Parse.Int(data["padLeft"]) : layoutGroup.padding.left, data.ContainsKey("padRight") ? Parse.Int(data["padRight"]) : layoutGroup.padding.right, data.ContainsKey("padTop") ? Parse.Int(data["padTop"]) : layoutGroup.padding.top, data.ContainsKey("padBottom") ? Parse.Int(data["padBottom"]) : layoutGroup.padding.bottom);
            if (data.ContainsKey("childAlign"))
            {
                layoutGroup.childAlignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), data["childAlign"]);
            }
        }
    }
}
