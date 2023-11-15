using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

#if !GAME_VERSION_1_29_0
using System.Collections;
#endif

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

#if GAME_VERSION_1_29_0
                listSetting.values = values.GetValueAs<List<object>>();
#else
                listSetting.values = values.GetValueAs<IList>();
#endif
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
