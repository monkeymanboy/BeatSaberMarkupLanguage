using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(ListSliderSetting))]
    public class ListSliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "options", new[]{ "options", "choices" } },
            { "updateDuringDrag", Array.Empty<string>() },
            { "onDragStarted", Array.Empty<string>() },
            { "dragStartedEvent", Array.Empty<string>() },
            { "onDragReleased", Array.Empty<string>() },
            { "dragReleasedEvent", Array.Empty<string>() }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ListSliderSetting listSetting = componentType.component as ListSliderSetting;

            if (componentType.data.TryGetValue("options", out string options))
            {
                if (!parserParams.values.TryGetValue(options, out BSMLValue values))
                    throw new Exception("options '" + options + "' not found");

                listSetting.values = values.GetValue() as List<object>;
            }
            else
            {
                throw new Exception("list must have associated options");
            }
        }
    }
}
