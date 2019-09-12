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
            if (data.TryGetValue("pad", out string pad))
            {
                int padding = Parse.Int(pad);
                layoutGroup.padding = new RectOffset(padding, padding, padding, padding);
            }

            layoutGroup.padding = new RectOffset(data.TryGetValue("padLeft", out string padLeft) ? Parse.Int(padLeft) : layoutGroup.padding.left, data.TryGetValue("padRight", out string padRight) ? Parse.Int(padRight) : layoutGroup.padding.right, data.TryGetValue("padTop", out string padTop) ? Parse.Int(padTop) : layoutGroup.padding.top, data.TryGetValue("padBottom", out string padBottom) ? Parse.Int(padBottom) : layoutGroup.padding.bottom);
            if (data.TryGetValue("childAlign", out string childAlign))
                layoutGroup.childAlignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), childAlign);
        }
    }
}
