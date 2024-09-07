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
                ModalKeyboard modalKeyboard = componentType.Component as ModalKeyboard;
                if (componentType.Data.TryGetValue("clearOnOpen", out string clearOnOpen))
                {
                    modalKeyboard.ClearOnOpen = Parse.Bool(clearOnOpen);
                }

                if (componentType.Data.TryGetValue("value", out string value))
                {
                    if (!parserParams.Values.TryGetValue(value, out BSMLValue associatedValue))
                    {
                        throw new ValueNotFoundException(value, parserParams.Host);
                    }

                    modalKeyboard.AssociatedValue = associatedValue;
                }

                if (componentType.Data.TryGetValue("onEnter", out string onEnter))
                {
                    if (!parserParams.Actions.TryGetValue(onEnter, out BSMLAction onEnterAction))
                    {
                        throw new ActionNotFoundException(onEnter, parserParams.Host);
                    }

                    modalKeyboard.OnEnter = onEnterAction;
                }

                if (componentType.Data.TryGetValue("showEvent", out string showEvent))
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
