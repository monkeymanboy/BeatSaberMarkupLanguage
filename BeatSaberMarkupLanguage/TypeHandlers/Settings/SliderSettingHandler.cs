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
            SliderSetting sliderSetting = componentType.Component as SliderSetting;

            if (componentType.Data.TryGetValue("isInt", out string isInt))
            {
                sliderSetting.IsInt = Parse.Bool(isInt);
            }

            if (componentType.Data.TryGetValue("increment", out string increment))
            {
                sliderSetting.Increments = Parse.Float(increment);
            }

            if (componentType.Data.TryGetValue("minValue", out string minValue))
            {
                sliderSetting.Slider.minValue = Parse.Float(minValue);
            }

            if (componentType.Data.TryGetValue("maxValue", out string maxValue))
            {
                sliderSetting.Slider.maxValue = Parse.Float(maxValue);
            }

            if (componentType.Data.TryGetValue("showButtons", out string showButtons))
            {
                sliderSetting.ShowButtons = Parse.Bool(showButtons);
            }
        }
    }
}
