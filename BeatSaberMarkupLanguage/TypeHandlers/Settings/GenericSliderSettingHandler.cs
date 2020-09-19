using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(GenericSliderSetting))]
    public class GenericSliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "updateDuringDrag", Array.Empty<string>() }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            GenericSliderSetting sliderSetting = componentType.component as GenericSliderSetting;

            if (componentType.data.TryGetValue("updateDuringDrag", out string updateDuringDrag))
                sliderSetting.updateDuringDrag = Parse.Bool(updateDuringDrag);
            else
                sliderSetting.updateDuringDrag = true;
        }
    }
}
