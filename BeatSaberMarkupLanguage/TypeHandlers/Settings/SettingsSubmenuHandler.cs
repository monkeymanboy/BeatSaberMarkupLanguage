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

            if (data.TryGetValue("text", out string text))
                textMesh.text = text;

            if (data.TryGetValue("fontSize", out string fontSize))
                textMesh.fontSize = Parse.Float(fontSize);

            if (data.TryGetValue("alignment", out string alignment))
                textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), alignment);

            if (data.TryGetValue("overflowMode", out string overflowMode))
                textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), overflowMode);

            if (data.TryGetValue("hoverHint", out string hoverHint))
            {
                HoverHint hover = textMesh.gameObject.AddComponent<HoverHint>();
                hover.text = hoverHint;
                hover.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}
