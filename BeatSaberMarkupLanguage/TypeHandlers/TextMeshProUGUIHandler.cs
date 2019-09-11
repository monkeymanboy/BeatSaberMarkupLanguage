using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUIHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "fontSize", new[]{"font-size"} },
            { "alignment", new[]{"align"} },
            { "overflowMode", new[]{"overflow-mode"} },
            { "bold", new[]{"bold"} },
            { "italics", new[]{ "italics" } },
            { "underlined", new[]{ "underlined" } },
            { "strikethrough", new[]{ "strikethrough" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            TextMeshProUGUI textMesh = obj as TextMeshProUGUI;
            if (data.ContainsKey("text"))
                textMesh.text = data["text"];

            if (data.ContainsKey("fontSize"))
                textMesh.fontSize = Parse.Float(data["fontSize"]);

            if (data.ContainsKey("alignment"))
                textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), data["alignment"]);

            if (data.ContainsKey("overflowMode"))
                textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), data["overflowMode"]);

            if (data.ContainsKey("bold") && Parse.Bool(data["bold"]))
                textMesh.text = $"<b>{textMesh.text}</b>";

            if (data.ContainsKey("italics") && Parse.Bool(data["italics"]))
                textMesh.text = $"<i>{textMesh.text}</i>";

            if (data.ContainsKey("underlined") && Parse.Bool(data["underlined"]))
                textMesh.text = $"<u>{textMesh.text}</u>";

            if (data.ContainsKey("strikethrough") && Parse.Bool(data["strikethrough"]))
                textMesh.text = $"<s>{textMesh.text}</s>";
        }
    }
}
