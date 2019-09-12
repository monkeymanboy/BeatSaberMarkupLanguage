using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            LayoutElement layoutElement = obj as LayoutElement;
            if (data.TryGetValue("ignoreLayout", out string ignoreLayout))
                layoutElement.ignoreLayout = Parse.Bool(ignoreLayout);

            if (data.TryGetValue("preferredWidth", out string preferredWidth))
                layoutElement.preferredWidth = Parse.Float(preferredWidth);

            if (data.TryGetValue("preferredHeight", out string preferredHeight))
                layoutElement.preferredHeight = Parse.Float(preferredHeight);
        }
    }
}
