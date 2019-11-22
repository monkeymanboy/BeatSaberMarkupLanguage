using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

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

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            TextMeshProUGUI textMesh = (componentType.component as SubmenuText).submenuText;

            if (componentType.data.TryGetValue("text", out string text))
                textMesh.text = text;

            if (componentType.data.TryGetValue("fontSize", out string fontSize))
                textMesh.fontSize = Parse.Float(fontSize);

            if (componentType.data.TryGetValue("alignment", out string alignment))
                textMesh.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), alignment);

            if (componentType.data.TryGetValue("overflowMode", out string overflowMode))
                textMesh.overflowMode = (TextOverflowModes)Enum.Parse(typeof(TextOverflowModes), overflowMode);

            if (componentType.data.TryGetValue("hoverHint", out string hoverHint))
            {
                HoverHint hover = textMesh.gameObject.AddComponent<HoverHint>();
                hover.text = hoverHint;
                hover.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}
