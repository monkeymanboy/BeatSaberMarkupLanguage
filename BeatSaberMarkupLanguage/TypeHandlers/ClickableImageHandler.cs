using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableImage))]
    public class ClickableImageHandler : TypeHandler<ClickableImage>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event", "event-click"} },
            { "highlightColor", new[]{ "highlight-color" } },
            { "defaultColor", new[]{ "default-color" } }
        };

        public override Dictionary<string, Action<ClickableImage, string>> Setters => new Dictionary<string, Action<ClickableImage, string>>()
        {
            { "highlightColor", new Action<ClickableImage, string>((image, color) => image.highlightColor = GetColor(color)) },
            { "defaultColor", new Action<ClickableImage, string>((image, color) => image.defaultColor = GetColor(color)) }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ClickableImage clickableImage = componentType.component as ClickableImage;
            if (componentType.data.TryGetValue("onClick", out string onClick))
            {
                clickableImage.OnClickEvent += delegate
                {
                    if (!parserParams.actions.TryGetValue(onClick, out BSMLAction onClickAction))
                        throw new Exception("on-click action '" + onClick + "' not found");

                    onClickAction.Invoke();
                };
            }

            if (componentType.data.TryGetValue("clickEvent", out string clickEvent))
            {
                clickableImage.OnClickEvent += delegate
                {
                    parserParams.EmitEvent(clickEvent);
                };
            }

            if (componentType.data.TryGetValue("defaultColor", out var defaultColor))
            {
                clickableImage.defaultColor = GetColor(defaultColor);
                clickableImage.color = clickableImage.defaultColor;
            }

            if (componentType.data.TryGetValue("highlightColor", out var highlightColor))
            {
                clickableImage.highlightColor = GetColor(highlightColor);
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
