using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableText))]
    public class ClickableTextHandler : TypeHandler<ClickableText>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "onClick", new[] { "on-click" } },
            { "clickEvent", new[] { "click-event", "event-click" } },
            { "highlightColor", new[] { "highlight-color" } },
            { "defaultColor", new[] { "default-color" } },
        };

        public override Dictionary<string, Action<ClickableText, string>> Setters => new()
        {
            { "highlightColor", new Action<ClickableText, string>((text, color) => text.HighlightColor = GetColor(color)) },
            { "defaultColor", new Action<ClickableText, string>((text, color) => text.DefaultColor = GetColor(color)) },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            ClickableText clickableText = componentType.component as ClickableText;
            if (componentType.data.TryGetValue("onClick", out string onClick))
            {
                clickableText.OnClickEvent += (eventData) =>
                {
                    if (!parserParams.actions.TryGetValue(onClick, out BSMLAction onClickAction))
                    {
                        throw new ActionNotFoundException(onClick, parserParams.host);
                    }

                    onClickAction.Invoke();
                };
            }

            if (componentType.data.TryGetValue("clickEvent", out string clickEvent))
            {
                clickableText.OnClickEvent += (eventData) =>
                {
                    parserParams.EmitEvent(clickEvent);
                };
            }
        }

        private static Color GetColor(string colorStr)
        {
            if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
            {
                return color;
            }

            Logger.Log?.Warn($"Color {colorStr}, is not a valid color.");
            return Color.white;
        }
    }
}
