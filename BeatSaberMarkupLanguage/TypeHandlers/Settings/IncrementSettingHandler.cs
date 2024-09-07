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
            IncrementSetting incrementSetting = componentType.Component as IncrementSetting;

            if (componentType.Data.TryGetValue("isInt", out string isInt))
            {
                incrementSetting.IsInt = Parse.Bool(isInt);
            }

            if (componentType.Data.TryGetValue("increment", out string increment))
            {
                incrementSetting.Increments = Parse.Float(increment);
            }

            if (componentType.Data.TryGetValue("minValue", out string minValue))
            {
                incrementSetting.MinValue = Parse.Float(minValue);
            }

            if (componentType.Data.TryGetValue("maxValue", out string maxValue))
            {
                incrementSetting.MaxValue = Parse.Float(maxValue);
            }
        }
    }
}
