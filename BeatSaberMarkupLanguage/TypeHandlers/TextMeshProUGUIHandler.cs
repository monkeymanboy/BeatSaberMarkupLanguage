using System;
using System.Collections.Generic;
using TMPro;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUIHandler : TypeHandler<TextMeshProUGUI>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "enableAutoSizing", new[] { "enable-auto-sizing" } },
            { "text", new[] { "text" } },
            { "fontSize", new[] { "font-size" } },
            { "fontSizeMin", new[] { "font-size-min" } },
            { "fontSizeMax", new[] { "font-size-max" } },
            { "color", new[] { "font-color", "color" } },
            { "faceColor", new[] { "face-color" } },
            { "outlineColor", new[] { "outline-color" } }, // Outline not supported for Teko fonts
            { "outlineWidth", new[] { "outline-width" } },
            { "richText", new[] { "rich-text" } }, // Enabled by default
            { "alignment", new[] { "font-align", "align" } },
            { "overflowMode", new[] { "overflow-mode" } },
            { "wordWrapping", new[] { "word-wrapping" } },
            { "bold", new[] { "bold" } },
            { "italics", new[] { "italics" } },
            { "underlined", new[] { "underlined" } },
            { "strikethrough", new[] { "strikethrough" } },
            { "allUppercase", new[] { "all-uppercase" } },
            { "lineSpacing", new[] { "line-spacing" } },
            { "lineSpacingAdjustment", new[] { "line-spacing-adjustment" } },
            { "paragraphSpacing", new[] { "paragraph-spacing" } },
            { "monochrome", new[] { "monochrome" } },
        };

        public override Dictionary<string, Action<TextMeshProUGUI, string>> Setters => new()
        {
            { "enableAutoSizing", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.enableAutoSizing = Parse.Bool(value)) },
            { "text", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.text = value) },
            { "fontSize", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontSize = Parse.Float(value)) },
            { "fontSizeMin", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontSizeMin = Parse.Float(value)) },
            { "fontSizeMax", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontSizeMax = Parse.Float(value)) },
            { "color", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.color = Parse.Color(value)) },
            { "faceColor", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.faceColor = Parse.Color(value)) },
            { "outlineColor", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.outlineColor = Parse.Color(value)) },
            { "outlineWidth", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.outlineWidth = Parse.Float(value)) },
            { "richText", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.richText = Parse.Bool(value)) },
            { "alignment", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), value)) },
            { "overflowMode", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), value)) },
            { "wordWrapping", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.textWrappingMode = Parse.Bool(value) ? TextWrappingModes.Normal : TextWrappingModes.NoWrap) },
            { "bold", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Bold, value)) },
            { "italics", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Italic, value)) },
            { "underlined", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Underline, value)) },
            { "strikethrough", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.Strikethrough, value)) },
            { "allUppercase", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.fontStyle = SetStyle(textMesh.fontStyle, FontStyles.UpperCase, value)) },
            { "lineSpacing", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.lineSpacing = Parse.Float(value)) },
            { "lineSpacingAdjustment", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.lineSpacingAdjustment = Parse.Float(value)) },
            { "paragraphSpacing", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.paragraphSpacing = Parse.Float(value)) },
            { "monochrome", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.font = Parse.Bool(value) ? BeatSaberUI.MonochromeTextFont : BeatSaberUI.MainTextFont) },
        };

        private static FontStyles SetStyle(FontStyles existing, FontStyles modifyStyle, string flag)
        {
            if (Parse.Bool(flag))
            {
                return existing |= modifyStyle;
            }
            else
            {
                return existing &= ~modifyStyle;
            }
        }
    }
}
