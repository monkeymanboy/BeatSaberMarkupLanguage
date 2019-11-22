using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUIHandler : TypeHandler<TextMeshProUGUI>
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

        public override Dictionary<string, Action<TextMeshProUGUI, string>> Setters => new Dictionary<string, Action<TextMeshProUGUI, string>>()
        {
            {"text", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.text = value) },
            {"fontSize", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.fontSize = Parse.Float(value)) },
            {"alignment", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), value)) },
            {"overflowMode", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), value)) },
            {"bold", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Bold, value)) },
            {"italics", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Italic, value))  },
            {"underlined", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Underline, value))  },
            {"strikethrough", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Strikethrough, value))  },
        };

        private static FontStyles SetStyle(FontStyles existing, FontStyles modifyStyle, string flag)
        {
            if (bool.TryParse(flag, out bool flagBool) && flagBool)
                return existing |= modifyStyle;
            else
                return existing &= ~modifyStyle;
        }
    }
}
