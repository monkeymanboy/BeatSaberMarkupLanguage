using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (data.ContainsKey("ignoreLayout"))
                layoutElement.ignoreLayout = Parse.Bool(data["ignoreLayout"]);
            if (data.ContainsKey("preferredWidth"))
                layoutElement.preferredWidth = Parse.Float(data["preferredWidth"]);
            if (data.ContainsKey("preferredHeight"))
                layoutElement.preferredHeight = Parse.Float(data["preferredHeight"]);
        }
    }
}
