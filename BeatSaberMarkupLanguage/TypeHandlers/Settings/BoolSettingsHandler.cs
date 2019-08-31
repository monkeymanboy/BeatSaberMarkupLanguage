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
    [ComponentHandler(typeof(BoolSetting))]
    public class BoolSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{ "text" } },
            { "onChange", new[]{ "on-change"} },
            { "value", new[]{ "value"} },
            { "initialValue", new[]{ "initial-value"} },
            { "setEvent", new[]{ "set-event"} },
            { "getEvent", new[]{ "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            BoolSetting boolSetting = obj as BoolSetting;
            if (data.ContainsKey("text"))
                boolSetting.LabelText = data["text"];
            if (data.ContainsKey("applyOnChange"))
                boolSetting.updateOnChange = Parse.Bool(data["applyOnChange"]);
            if (data.ContainsKey("initialValue"))
                boolSetting.Value = Parse.Bool(data["initialValue"]);
            if (data.ContainsKey("onChange"))
            {
                if (!parserParams.actions.ContainsKey(data["onChange"]))
                    throw new Exception("on-change action '" + data["onChange"] + "' not found");
                boolSetting.onChange = parserParams.actions[data["onChange"]];
            }
            if (data.ContainsKey("value"))
            {
                if (!parserParams.values.ContainsKey(data["value"]))
                    throw new Exception("value '" + data["value"] + "' not found");
                boolSetting.associatedValue = parserParams.values[data["value"]];
            }
            parserParams.AddEvent(data.ContainsKey("setEvent") ? data["setEvent"] : "apply", boolSetting.ApplyValue);
            parserParams.AddEvent(data.ContainsKey("getEvent") ? data["getEvent"] : "cancel", boolSetting.ReceiveValue);
            boolSetting.Setup();
        }
    }
}
