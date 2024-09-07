using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
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
            { "highlightColor", new Action<ClickableText, string>((text, color) => text.HighlightColor = Parse.Color(color)) },
            { "defaultColor", new Action<ClickableText, string>((text, color) => text.DefaultColor = Parse.Color(color)) },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            ClickableText clickableText = componentType.Component as ClickableText;
            if (componentType.Data.TryGetValue("onClick", out string onClick))
            {
                clickableText.OnClickEvent += (eventData) =>
                {
                    if (!parserParams.Actions.TryGetValue(onClick, out BSMLAction onClickAction))
                    {
                        throw new ActionNotFoundException(onClick, parserParams.Host);
                    }

                    onClickAction.Invoke();
                };
            }

            if (componentType.Data.TryGetValue("clickEvent", out string clickEvent))
            {
                clickableText.OnClickEvent += (eventData) =>
                {
                    parserParams.EmitEvent(clickEvent);
                };
            }
        }
    }
}
