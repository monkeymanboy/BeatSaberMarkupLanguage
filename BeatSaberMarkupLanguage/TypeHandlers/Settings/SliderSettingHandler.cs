using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(SliderSetting))]
    public class SliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "increment", new[] { "increment" } },
            { "minValue", new[] { "min" } },
            { "maxValue", new[] { "max" } },
            { "isInt", new[] { "integer-only" } },
            { "showButtons", new[] { "show-buttons" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            SliderSetting sliderSetting = componentType.component as SliderSetting;

            if (componentType.data.TryGetValue("isInt", out string isInt))
            {
                sliderSetting.isInt = Parse.Bool(isInt);
            }

            if (componentType.data.TryGetValue("increment", out string increment))
            {
                sliderSetting.increments = Parse.Float(increment);
            }

            if (componentType.data.TryGetValue("minValue", out string minValue))
            {
                sliderSetting.slider.minValue = Parse.Float(minValue);
            }

            if (componentType.data.TryGetValue("maxValue", out string maxValue))
            {
                sliderSetting.slider.maxValue = Parse.Float(maxValue);
            }

            if (componentType.data.TryGetValue("showButtons", out string showButtons))
            {
                sliderSetting.showButtons = Parse.Bool(showButtons);
            }
        }
    }
}
