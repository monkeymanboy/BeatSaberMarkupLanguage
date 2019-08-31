using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (data.ContainsKey("text"))
                sliderSetting.LabelText = data["text"];
            if (data.ContainsKey("formatter"))
                sliderSetting.formatter = parserParams.actions[data["formatter"]];
            if (data.ContainsKey("applyOnChange"))
                sliderSetting.updateOnChange = bool.Parse(data["applyOnChange"]);
            if (data.ContainsKey("isInt"))
                sliderSetting.isInt = bool.Parse(data["isInt"]);
            if (data.ContainsKey("increment"))
                sliderSetting.increments = float.Parse(data["increment"]);
            if (data.ContainsKey("minValue"))
                sliderSetting.slider.minValue = float.Parse(data["minValue"]);
            if (data.ContainsKey("maxValue"))
                sliderSetting.slider.maxValue = float.Parse(data["maxValue"]);
            if (data.ContainsKey("onChange"))
            {
                if (!parserParams.actions.ContainsKey(data["onChange"]))
                    throw new Exception("on-change action '" + data["onChange"] + "' not found");
                sliderSetting.onChange = parserParams.actions[data["onChange"]];
            }
            if (data.ContainsKey("value"))
            {
                if (!parserParams.values.ContainsKey(data["value"]))
                    throw new Exception("value '" + data["value"] + "' not found");
                sliderSetting.associatedValue = parserParams.values[data["value"]];
            }
            parserParams.AddEvent(data.ContainsKey("setEvent") ? data["setEvent"] : "apply", sliderSetting.ApplyValue);
            parserParams.AddEvent(data.ContainsKey("getEvent") ? data["getEvent"] : "cancel", sliderSetting.ReceiveValue);
            sliderSetting.Setup();
        }
    }
}
