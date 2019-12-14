using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(BoolSetting))]
    public class BoolSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onChange", new[]{ "on-change"} },
            { "value", new[]{ "value"} },
            { "initialValue", new[]{ "initial-value"} },
            { "setEvent", new[]{ "set-event"} },
            { "getEvent", new[]{ "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            BoolSetting boolSetting = componentType.component as BoolSetting;

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
                boolSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (componentType.data.TryGetValue("initialValue", out string initialValue))
                boolSetting.Value = Parse.Bool(initialValue);

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                boolSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                boolSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(componentType.data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", boolSetting.ApplyValue);
            parserParams.AddEvent(componentType.data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", boolSetting.ReceiveValue);

            boolSetting.Setup();
        }
    }
}
