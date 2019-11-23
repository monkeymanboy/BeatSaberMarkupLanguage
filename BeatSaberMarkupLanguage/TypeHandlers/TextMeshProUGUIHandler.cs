using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
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
            { "font", new[]{ "font" } },
            { "color", new[]{ "color" } },
            { "faceColor", new[]{ "faceColor" } },
            { "outlineColor", new[]{ "outlineColor" } }, // Outline not supported for Teko fonts
            { "outlineWidth", new[]{ "outlineWidth" } },
            { "richText", new[]{ "richText" } },
            { "alignment", new[]{"align"} },
            { "overflowMode", new[]{"overflow-mode"} },
            { "bold", new[]{"bold"} },
            { "italics", new[]{ "italics" } },
            { "underlined", new[]{ "underlined" } },
            { "strikethrough", new[]{ "strikethrough" } }
        };
        public override Dictionary<string, Action<TextMeshProUGUI, string>> Setters => _setters;
        private Dictionary<string, Action<TextMeshProUGUI, string>> _setters = new Dictionary<string, Action<TextMeshProUGUI, string>>()
        {
            {"text", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.text = value) },
            {"fontSize", new Action<TextMeshProUGUI,string>((textMesh, value) => textMesh.fontSize = Parse.Float(value)) },
            {"font", new Action<TextMeshProUGUI,string>(SetFont) },
            {"color", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.color = GetColor(value)) },
            {"faceColor", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.faceColor = GetColor(value)) },
            {"outlineColor", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.outlineColor = GetColor(value)) },
            {"outlineWidth", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.outlineWidth = Parse.Float(value)) },
            {"richText", new Action<TextMeshProUGUI, string>((textMesh, value) => textMesh.richText = Parse.Bool(value)) },
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

        private static void SetFont(TextMeshProUGUI textMesh, string fontName)
        {

            TMP_FontAsset fontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Where(t => 
                string.Equals(t.name, fontName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (fontAsset != null)
            {
                textMesh.gameObject.SetActive(false);
                MonoBehaviour.DestroyImmediate(textMesh.font);
                textMesh.font = MonoBehaviour.Instantiate(fontAsset);
                textMesh.gameObject.SetActive(true);
            }
            else
            {
                var fontList = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                Logger.log?.Warn($"Font {fontName} not found. Available fonts: {string.Join(", ", fontList?.Where(f => !f.name.Contains("(Clone)")).Select(f => f.name))}");
            }
        }

        private static Color GetColor(string colorStr)
        {
            if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                return color;
            return (Color)Enum.Parse(typeof(Color), colorStr);
        }
    }
}
