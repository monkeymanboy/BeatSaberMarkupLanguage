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
            if (data.TryGetValue("text", out string text))
                textMesh.text = text;

            if (data.TryGetValue("fontSize", out string fontSize))
                textMesh.fontSize = Parse.Float(fontSize);

            if (data.TryGetValue("alignment", out string alignment))
                textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), alignment);

            if (data.TryGetValue("overflowMode", out string overflowMode))
                textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), overflowMode);

            if (data.TryGetValue("bold", out string bold) && Parse.Bool(bold))
                textMesh.text = $"<b>{textMesh.text}</b>";

            if (data.TryGetValue("italics", out string italics) && Parse.Bool(italics))
                textMesh.text = $"<i>{textMesh.text}</i>";

            if (data.TryGetValue("underlined", out string underlined) && Parse.Bool(underlined))
                textMesh.text = $"<u>{textMesh.text}</u>";

            if (data.TryGetValue("strikethrough", out string strikethrough) && Parse.Bool(strikethrough))
                textMesh.text = $"<s>{textMesh.text}</s>";
        }
    }
}
