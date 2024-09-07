using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(ListSliderSetting))]
    public class ListSliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "options", new[] { "options", "choices" } },
            { "showButtons", new[] { "show-buttons" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ListSliderSetting listSetting = componentType.Component as ListSliderSetting;

            if (componentType.Data.TryGetValue("options", out string options))
            {
                if (!parserParams.Values.TryGetValue(options, out BSMLValue values))
                {
                    throw new ValueNotFoundException(options, parserParams.Host);
                }

                listSetting.Values = values.GetValueAs<IList>();
            }
            else
            {
                throw new MissingAttributeException(this, "options");
            }

            if (componentType.Data.TryGetValue("showButtons", out string showButtons))
            {
                listSetting.ShowButtons = Parse.Bool(showButtons);
            }
        }
    }
}
