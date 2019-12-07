using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(IncrementSetting))]
    public class IncrementSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onChange", new[] { "on-change"} },
            { "value", new[] { "value"} },
            { "setEvent", new[] { "set-event"} },
            { "getEvent", new[] { "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } },
            { "increment", new[] { "increment" } },
            { "minValue", new[] { "min" } },
            { "maxValue", new[] { "max" } },
            { "isInt", new[] { "integer-only" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            IncrementSetting incrementSetting = componentType.component as IncrementSetting;

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
                incrementSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (componentType.data.TryGetValue("isInt", out string isInt))
                incrementSetting.isInt = Parse.Bool(isInt);

            if (componentType.data.TryGetValue("increment", out string increment))
                incrementSetting.increments = Parse.Float(increment);

            if (componentType.data.TryGetValue("minValue", out string minValue))
                incrementSetting.minValue = Parse.Float(minValue);

            if (componentType.data.TryGetValue("maxValue", out string maxValue))
                incrementSetting.maxValue = Parse.Float(maxValue);

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                incrementSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                incrementSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(componentType.data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", incrementSetting.ApplyValue);
            parserParams.AddEvent(componentType.data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", incrementSetting.ReceiveValue);

            incrementSetting.Setup();
        }
    }
}
