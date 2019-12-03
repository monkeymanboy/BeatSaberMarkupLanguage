using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(SliderSetting))]
    public class SliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onChange", new[]{ "on-change"} },
            { "value", new[]{ "value"} },
            { "initialValue", new[]{ "initial-value"} },
            { "setEvent", new[]{ "set-event"} },
            { "getEvent", new[]{ "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } },
            { "increment", new[] { "increment" } },
            { "minValue", new[] { "min" } },
            { "maxValue", new[] { "max" } },
            { "isInt", new[] { "integer-only" } },
            { "formatter", new[] { "formatter" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            SliderSetting sliderSetting = componentType.component as SliderSetting;

            if (componentType.data.TryGetValue("formatter", out string formatter))
                sliderSetting.formatter = parserParams.actions[formatter];

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
                sliderSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (componentType.data.TryGetValue("isInt", out string isInt))
                sliderSetting.isInt = Parse.Bool(isInt);

            if (componentType.data.TryGetValue("increment", out string increment))
                sliderSetting.increments = Parse.Float(increment);

            if (componentType.data.TryGetValue("minValue", out string minValue))
                sliderSetting.slider.minValue = Parse.Float(minValue);

            if (componentType.data.TryGetValue("maxValue", out string maxValue))
                sliderSetting.slider.maxValue = Parse.Float(maxValue);

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                sliderSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                sliderSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(componentType.data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", sliderSetting.ApplyValue);
            parserParams.AddEvent(componentType.data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", sliderSetting.ReceiveValue);

            sliderSetting.Setup();
        }
    }
}
