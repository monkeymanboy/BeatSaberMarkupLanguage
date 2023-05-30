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
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "padTop", new[] { "pad-top" } },
            { "padBottom", new[] { "pad-bottom" } },
            { "padLeft", new[] { "pad-left" } },
            { "padRight", new[] { "pad-right" } },
            { "pad", new[] { "pad" } },
            { "childAlign", new[] { "child-align" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            LayoutGroup layoutGroup = (LayoutGroup)componentType.component;
            if (componentType.data.TryGetValue("pad", out string pad))
            {
                int padding = Parse.Int(pad);
                layoutGroup.padding = new RectOffset(padding, padding, padding, padding);
            }

            layoutGroup.padding = new RectOffset(componentType.data.TryGetValue("padLeft", out string padLeft) ? Parse.Int(padLeft) : layoutGroup.padding.left, componentType.data.TryGetValue("padRight", out string padRight) ? Parse.Int(padRight) : layoutGroup.padding.right, componentType.data.TryGetValue("padTop", out string padTop) ? Parse.Int(padTop) : layoutGroup.padding.top, componentType.data.TryGetValue("padBottom", out string padBottom) ? Parse.Int(padBottom) : layoutGroup.padding.bottom);
            if (componentType.data.TryGetValue("childAlign", out string childAlign))
            {
                layoutGroup.childAlignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), childAlign);
            }
        }
    }
}
