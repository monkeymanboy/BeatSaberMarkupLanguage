using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableImage))]
    public class ClickableImageHandler : TypeHandler<ClickableImage>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "onClick", new[] { "on-click" } },
            { "clickEvent", new[] { "click-event", "event-click" } },
            { "highlightColor", new[] { "highlight-color" } },
            { "defaultColor", new[] { "default-color" } },
        };

        public override Dictionary<string, Action<ClickableImage, string>> Setters => new()
        {
            { "highlightColor", new Action<ClickableImage, string>((image, color) => image.HighlightColor = Parse.Color(color)) },
            { "defaultColor", new Action<ClickableImage, string>((image, color) => image.DefaultColor = Parse.Color(color)) },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            ClickableImage clickableImage = componentType.component as ClickableImage;
            if (componentType.data.TryGetValue("onClick", out string onClick))
            {
                clickableImage.OnClickEvent += (eventData) =>
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
                clickableImage.OnClickEvent += (eventData) =>
                {
                    parserParams.EmitEvent(clickEvent);
                };
            }
        }
    }
}
