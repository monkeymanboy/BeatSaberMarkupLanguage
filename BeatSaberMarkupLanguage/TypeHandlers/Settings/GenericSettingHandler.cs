using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(GenericSetting))]
    public class GenericSettingHandler : TypeHandler<GenericSetting>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "onChange", new[] { "on-change" } },
            { "value", new[] { "value" } },
            { "setEvent", new[] { "set-event" } },
            { "getEvent", new[] { "get-event" } },
            { "applyOnChange", new[] { "apply-on-change" } },
            { "formatter", new[] { "formatter" } },
            { "bindValue", new[] { "bind-value" } },
        };

        public override Dictionary<string, Action<GenericSetting, string>> Setters => new();

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            GenericSetting genericSetting = componentType.component as GenericSetting;

            if (componentType.data.TryGetValue("formatter", out string formatterId))
            {
                if (!parserParams.actions.TryGetValue(formatterId, out BSMLAction formatter))
                {
                    throw new ActionNotFoundException(formatterId, parserParams.host);
                }

                genericSetting.formatter = formatter;
            }

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
            {
                genericSetting.updateOnChange = Parse.Bool(applyOnChange);
            }

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                {
                    throw new ActionNotFoundException(onChange, parserParams.host);
                }

                genericSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                {
                    throw new ValueNotFoundException(value, parserParams.host);
                }

                genericSetting.associatedValue = associatedValue;
                if (componentType.data.TryGetValue("bindValue", out string bindValue) && Parse.Bool(bindValue))
                {
                    BindValue(componentType, parserParams, associatedValue, _ => genericSetting.ReceiveValue());
                }
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
