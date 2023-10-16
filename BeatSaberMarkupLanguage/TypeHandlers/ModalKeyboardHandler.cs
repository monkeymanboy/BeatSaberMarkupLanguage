using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ModalKeyboard))]
    public class ModalKeyboardHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "showEvent", new[] { "show-event" } },
            { "value", new[] { "value" } },
            { "onEnter", new[] { "on-enter" } },
            { "clearOnOpen", new[] { "clear-on-open" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                ModalKeyboard modalKeyboard = componentType.component as ModalKeyboard;
                if (componentType.data.TryGetValue("clearOnOpen", out string clearOnOpen))
                {
                    modalKeyboard.clearOnOpen = bool.Parse(clearOnOpen);
                }

                if (componentType.data.TryGetValue("value", out string value))
                {
                    if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    {
                        throw new ValueNotFoundException(value, parserParams.host);
                    }

                    modalKeyboard.associatedValue = associatedValue;
                }

                if (componentType.data.TryGetValue("onEnter", out string onEnter))
                {
                    if (!parserParams.actions.TryGetValue(onEnter, out BSMLAction onEnterAction))
                    {
                        throw new ActionNotFoundException(onEnter, parserParams.host);
                    }

                    modalKeyboard.onEnter = onEnterAction;
                }

                if (componentType.data.TryGetValue("showEvent", out string showEvent))
                {
                    parserParams.AddEvent(showEvent, () => modalKeyboard.ReceiveValue());
                }
            }
            catch (Exception ex)
            {
                Logger.Log?.Error(ex);
            }
        }
    }
}
