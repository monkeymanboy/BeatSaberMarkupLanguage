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
            ListSliderSetting listSetting = componentType.component as ListSliderSetting;

            if (componentType.data.TryGetValue("options", out string options))
            {
                if (!parserParams.values.TryGetValue(options, out BSMLValue values))
                {
                    throw new ValueNotFoundException(options, parserParams.host);
                }

                listSetting.values = values.GetValueAs<IList>();
            }
            else
            {
                throw new MissingAttributeException(this, "options");
            }

            if (componentType.data.TryGetValue("showButtons", out string showButtons))
            {
                listSetting.showButtons = Parse.Bool(showButtons);
            }
        }
    }
}
