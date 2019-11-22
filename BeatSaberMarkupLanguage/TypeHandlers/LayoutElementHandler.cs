using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LayoutElement))]
    public class LayoutElementHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "ignoreLayout", new[]{"ignore-layout"} },
            { "preferredWidth", new[]{"preferred-width", "pref-width"} },
            { "preferredHeight", new[]{"preferred-height", "pref-height"} }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            LayoutElement layoutElement = componentType.component as LayoutElement;
            if (componentType.data.TryGetValue("ignoreLayout", out string ignoreLayout))
                layoutElement.ignoreLayout = Parse.Bool(ignoreLayout);

            if (componentType.data.TryGetValue("preferredWidth", out string preferredWidth))
                layoutElement.preferredWidth = Parse.Float(preferredWidth);

            if (componentType.data.TryGetValue("preferredHeight", out string preferredHeight))
                layoutElement.preferredHeight = Parse.Float(preferredHeight);
        }
    }
}
