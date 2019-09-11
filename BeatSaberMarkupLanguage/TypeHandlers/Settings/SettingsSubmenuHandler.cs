using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(SubmenuText))]
    public class SettingsSubmenuHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "fontSize", new[]{"font-size"} },
            { "alignment", new[]{"align"} },
            { "overflowMode", new[]{"overflow-mode"} },
            { "hoverHint", new[]{ "hover-hint" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            TextMeshProUGUI textMesh = (obj as SubmenuText).submenuText;

            if (data.ContainsKey("text"))
                textMesh.text = data["text"];

            if (data.ContainsKey("fontSize"))
                textMesh.fontSize = Parse.Float(data["fontSize"]);

            if (data.ContainsKey("alignment"))
                textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), data["alignment"]);

            if (data.ContainsKey("overflowMode"))
                textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), data["overflowMode"]);

            if (data.ContainsKey("hoverHint"))
            {
                HoverHint hover = textMesh.gameObject.AddComponent<HoverHint>();
                hover.text = data["hoverHint"];
                hover.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}
