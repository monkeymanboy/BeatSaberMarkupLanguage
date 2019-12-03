using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ClickableText))]
    public class ClickableTextHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event", "event-click"} }
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
        }
    }
}
