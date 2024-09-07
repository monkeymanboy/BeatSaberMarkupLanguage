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
            GenericSetting genericSetting = componentType.Component as GenericSetting;

            if (componentType.Data.TryGetValue("formatter", out string formatterId))
            {
                if (!parserParams.Actions.TryGetValue(formatterId, out BSMLAction formatter))
                {
                    throw new ActionNotFoundException(formatterId, parserParams.Host);
                }

                genericSetting.Formatter = formatter;
            }

            if (componentType.Data.TryGetValue("applyOnChange", out string applyOnChange))
            {
                genericSetting.UpdateOnChange = Parse.Bool(applyOnChange);
            }

            if (componentType.Data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.Actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                {
                    throw new ActionNotFoundException(onChange, parserParams.Host);
                }

                genericSetting.OnChange = onChangeAction;
            }

            if (componentType.Data.TryGetValue("value", out string value))
            {
                if (!parserParams.Values.TryGetValue(value, out BSMLValue associatedValue))
                {
                    throw new ValueNotFoundException(value, parserParams.Host);
                }

                genericSetting.AssociatedValue = associatedValue;
                if (componentType.Data.TryGetValue("bindValue", out string bindValue) && Parse.Bool(bindValue))
                {
                    BindValue(componentType, parserParams, associatedValue, _ => genericSetting.ReceiveValue());
                }
            }

            parserParams.AddEvent(componentType.Data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", genericSetting.ApplyValue);
            parserParams.AddEvent(componentType.Data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", genericSetting.ReceiveValue);
        }

        public override void HandleTypeAfterChildren(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            (componentType.Component as GenericSetting).Setup();
        }
    }
}
