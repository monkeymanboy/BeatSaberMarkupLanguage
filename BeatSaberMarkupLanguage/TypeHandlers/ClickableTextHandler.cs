using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableText))]
    public class ClickableTextHandler : TypeHandler<ClickableText>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event", "event-click"} },
            { "highlightColor", new[]{ "highlight-color" } },
            { "defaultColor", new[]{ "default-color" } }
        };

        public override Dictionary<string, Action<ClickableText, string>> Setters => new Dictionary<string, Action<ClickableText, string>>()
        {
            { "highlightColor", new Action<ClickableText, string>((text, color) => text.highlightColor = GetColor(color)) },
            { "defaultColor", new Action<ClickableText, string>((text, color) => text.defaultColor = GetColor(color)) }

        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ClickableText clickableText = componentType.component as ClickableText;
            if (componentType.data.TryGetValue("onClick", out string onClick))
            {
                clickableText.OnClickEvent += delegate
                {
                    if (!parserParams.actions.TryGetValue(onClick, out BSMLAction onClickAction))
                        throw new Exception("on-click action '" + onClick + "' not found");

                    onClickAction.Invoke();
                };
            }

            if (componentType.data.TryGetValue("clickEvent", out string clickEvent))
            {
                clickableText.OnClickEvent += delegate
                {
                    parserParams.EmitEvent(clickEvent);
                };
            }

            if (componentType.data.TryGetValue("defaultColor", out var defaultColor))
            {
                clickableText.defaultColor = GetColor(defaultColor);
                clickableText.color = clickableText.defaultColor;
            }

            if (componentType.data.TryGetValue("highlightColor", out var highlightColor))
            {
                clickableText.highlightColor = GetColor(highlightColor);
            }
        }

        private static Color GetColor(string colorStr)
        {
            if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                return color;
            Logger.log?.Warn($"Color {colorStr}, is not a valid color.");
            return Color.white;
        }
    }
}
