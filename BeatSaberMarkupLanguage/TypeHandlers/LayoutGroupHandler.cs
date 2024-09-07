using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LayoutGroup))]
    public class LayoutGroupHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "padTop", new[] { "pad-top" } },
            { "padBottom", new[] { "pad-bottom" } },
            { "padLeft", new[] { "pad-left" } },
            { "padRight", new[] { "pad-right" } },
            { "pad", new[] { "padding", "pad" } },
            { "childAlign", new[] { "child-alignment", "child-align" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            LayoutGroup layoutGroup = (LayoutGroup)componentType.Component;
            if (componentType.Data.TryGetValue("pad", out string pad))
            {
                layoutGroup.padding = Parse.RectOffset(pad);
            }

            layoutGroup.padding = new RectOffset(
                componentType.Data.TryGetValue("padLeft", out string padLeft) ? Parse.Int(padLeft) : layoutGroup.padding.left,
                componentType.Data.TryGetValue("padRight", out string padRight) ? Parse.Int(padRight) : layoutGroup.padding.right,
                componentType.Data.TryGetValue("padTop", out string padTop) ? Parse.Int(padTop) : layoutGroup.padding.top,
                componentType.Data.TryGetValue("padBottom", out string padBottom) ? Parse.Int(padBottom) : layoutGroup.padding.bottom);

            if (componentType.Data.TryGetValue("childAlign", out string childAlign))
            {
                layoutGroup.childAlignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), childAlign);
            }
        }
    }
}
