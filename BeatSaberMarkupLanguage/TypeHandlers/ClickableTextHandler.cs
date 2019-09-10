using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableText))]
    public class ClickableTextHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            ClickableText clickableText = obj as ClickableText;
            if (data.ContainsKey("onClick"))
            {
                clickableText.OnClickEvent += delegate
                {
                    if (!parserParams.actions.ContainsKey(data["onClick"]))
                    {
                        throw new Exception("on-click action '" + data["onClick"] + "' not found");
                    }

                    parserParams.actions[data["onClick"]].Invoke();
                };
            }

            if (data.ContainsKey("clickEvent"))
            {
                clickableText.OnClickEvent += delegate
                {
                    parserParams.EmitEvent(data["clickEvent"]);
                };
            }
        }
    }
}
