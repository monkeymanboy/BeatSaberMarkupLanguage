using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(SliderSetting))]
    public class SliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{ "text" } },
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

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            SliderSetting sliderSetting = obj as SliderSetting;

            if (data.TryGetValue("text", out string text))
                sliderSetting.LabelText = text;

            if (data.TryGetValue("formatter", out string formatter))
                sliderSetting.formatter = parserParams.actions[formatter];

            if (data.TryGetValue("applyOnChange", out string applyOnChange))
                sliderSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (data.TryGetValue("isInt", out string isInt))
                sliderSetting.isInt = Parse.Bool(isInt);

            if (data.TryGetValue("increment", out string increment))
                sliderSetting.increments = Parse.Float(increment);

            if (data.TryGetValue("minValue", out string minValue))
                sliderSetting.slider.minValue = Parse.Float(minValue);

            if (data.TryGetValue("maxValue", out string maxValue))
                sliderSetting.slider.maxValue = Parse.Float(maxValue);

            if (data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                sliderSetting.onChange = onChangeAction;
            }

            if (data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                sliderSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", sliderSetting.ApplyValue);
            parserParams.AddEvent(data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", sliderSetting.ReceiveValue);

            sliderSetting.Setup();
        }
    }
}
