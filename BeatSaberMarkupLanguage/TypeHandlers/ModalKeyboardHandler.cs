using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ModalKeyboard))]
    public class ModalKeyboardHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "value", new[]{ "value" } },
            { "onEnter", new[]{ "on-enter" } },
            { "clearOnOpen", new[]{ "clear-on-open" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                ModalKeyboard modalKeyboard = componentType.component as ModalKeyboard;
                if (componentType.data.TryGetValue("clearOnOpen", out string clearOnOpen))
                    modalKeyboard.clearOnOpen = bool.Parse(clearOnOpen);

                if (componentType.data.TryGetValue("value", out string value))
                {
                    if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                        throw new Exception("value '" + value + "' not found");

                    modalKeyboard.associatedValue = associatedValue;
                }

                if (componentType.data.TryGetValue("onEnter", out string onEnter))
                {
                    if (!parserParams.actions.TryGetValue(onEnter, out BSMLAction onEnterAction))
                        throw new Exception("on-enter action '" + onEnter + "' not found");

                    modalKeyboard.onEnter = onEnterAction;
                }
            }
            catch (Exception ex)
            {
                Logger.log?.Error(ex);
            }
        }
    }
}
