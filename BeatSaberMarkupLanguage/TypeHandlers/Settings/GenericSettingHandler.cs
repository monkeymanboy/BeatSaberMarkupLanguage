using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(GenericSetting))]
    public class GenericSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onChange", new[] { "on-change"} },
            { "value", new[] { "value"} },
            { "setEvent", new[] { "set-event"} },
            { "getEvent", new[] { "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } },
            { "formatter", new[] { "formatter" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            GenericSetting genericSetting = componentType.component as GenericSetting;

            if (componentType.data.TryGetValue("formatter", out string formatterId))
            {
                if (!parserParams.actions.TryGetValue(formatterId, out BSMLAction formatter))
                    throw new Exception("formatter action '" + formatter + "' not found");

                genericSetting.formatter = formatter;
            }

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
                genericSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                genericSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                genericSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(componentType.data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", genericSetting.ApplyValue);
            parserParams.AddEvent(componentType.data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", genericSetting.ReceiveValue);
        }

        public override void HandleTypeAfterChildren(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            (componentType.component as GenericSetting).Setup();
        }
    }
}
