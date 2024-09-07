using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(ListSetting))]
    public class ListSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "options", new[] { "options", "choices" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ListSetting listSetting = componentType.Component as ListSetting;
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
        }
    }
}
