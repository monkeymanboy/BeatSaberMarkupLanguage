using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(CheckboxSetting))]
    public class CheckboxSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onChange", new[]{ "on-change" } },
            { "value", new[]{ "value" } },
            { "initialValue", new[]{ "initial-value" } },
            { "setEvent", new[]{ "set-event"} },
            { "getEvent", new[]{ "get-event"} },
            { "applyOnChange", new[] { "apply-on-change" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            CheckboxSetting checkboxSetting = componentType.component as CheckboxSetting;

            if (componentType.data.TryGetValue("applyOnChange", out string applyOnChange))
                checkboxSetting.updateOnChange = Parse.Bool(applyOnChange);

            if (componentType.data.TryGetValue("initialValue", out string initialValue))
                checkboxSetting.CheckboxValue = Parse.Bool(initialValue);

            if (componentType.data.TryGetValue("onChange", out string onChange))
            {
                if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                    throw new Exception("on-change action '" + onChange + "' not found");

                checkboxSetting.onChange = onChangeAction;
            }

            if (componentType.data.TryGetValue("value", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    throw new Exception("value '" + value + "' not found");

                checkboxSetting.associatedValue = associatedValue;
            }

            parserParams.AddEvent(componentType.data.TryGetValue("setEvent", out string setEvent) ? setEvent : "apply", checkboxSetting.ApplyValue);
            parserParams.AddEvent(componentType.data.TryGetValue("getEvent", out string getEvent) ? getEvent : "cancel", checkboxSetting.ReceiveValue);

            checkboxSetting.Setup();
        }
    }
}
