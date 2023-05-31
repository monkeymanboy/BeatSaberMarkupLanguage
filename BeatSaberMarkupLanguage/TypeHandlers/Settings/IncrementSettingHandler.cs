using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(IncrementSetting))]
    public class IncrementSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "increment", new[] { "increment" } },
            { "minValue", new[] { "min" } },
            { "maxValue", new[] { "max" } },
            { "isInt", new[] { "integer-only" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            IncrementSetting incrementSetting = componentType.component as IncrementSetting;

            if (componentType.data.TryGetValue("isInt", out string isInt))
            {
                incrementSetting.isInt = Parse.Bool(isInt);
            }

            if (componentType.data.TryGetValue("increment", out string increment))
            {
                incrementSetting.increments = Parse.Float(increment);
            }

            if (componentType.data.TryGetValue("minValue", out string minValue))
            {
                incrementSetting.minValue = Parse.Float(minValue);
            }

            if (componentType.data.TryGetValue("maxValue", out string maxValue))
            {
                incrementSetting.maxValue = Parse.Float(maxValue);
            }
        }
    }
}
