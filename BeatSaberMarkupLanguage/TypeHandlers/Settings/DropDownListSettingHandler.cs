using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(DropDownListSetting))]
    public class DropDownListSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{ "text" } },
            { "onChange", new[]{ "on-change"} },
            { "value", new[]{ "value"} },
            { "setEvent", new[]{ "set-event"} },
            { "getEvent", new[]{ "get-event"} },
            { "options", new[]{ "options", "choices" } },
            { "applyOnChange", new[] { "apply-on-change" } },
            { "formatter", new[] { "formatter" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            DropDownListSetting listSetting = obj as DropDownListSetting;

            if (data.ContainsKey("text"))
            {
                listSetting.dropdown.SetLabelText(data["text"]);
            }

            if (data.ContainsKey("applyOnChange"))
            {
                listSetting.updateOnChange = Parse.Bool(data["applyOnChange"]);
            }

            if (data.ContainsKey("formatter"))
            {
                listSetting.formatter = parserParams.actions[data["formatter"]];
            }

            if (data.ContainsKey("onChange"))
            {
                if (!parserParams.actions.ContainsKey(data["onChange"]))
                {
                    throw new Exception("on-change action '" + data["onChange"] + "' not found");
                }

                listSetting.onChange = parserParams.actions[data["onChange"]];
            }

            if (data.ContainsKey("value"))
            {
                if (!parserParams.values.ContainsKey(data["value"]))
                {
                    throw new Exception("value '" + data["value"] + "' not found");
                }

                listSetting.associatedValue = parserParams.values[data["value"]];
            }

            if (data.ContainsKey("options"))
            {
                if (!parserParams.values.ContainsKey(data["options"]))
                {
                    throw new Exception("options '" + data["options"] + "' not found");
                }

                listSetting.values = parserParams.values[data["options"]].GetValue() as List<object>;
            }
            else
            {
                throw new Exception("list must have associated options");
            }

            parserParams.AddEvent(data.ContainsKey("setEvent") ? data["setEvent"] : "apply", listSetting.ApplyValue);
            parserParams.AddEvent(data.ContainsKey("getEvent") ? data["getEvent"] : "cancel", listSetting.ReceiveValue);

            listSetting.Setup();
            listSetting.dropdown.ReloadData();
            listSetting.gameObject.SetActive(true);
        }
    }
}
