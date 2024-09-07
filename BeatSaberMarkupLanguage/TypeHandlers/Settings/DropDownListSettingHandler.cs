using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(DropDownListSetting))]
    public class DropDownListSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "options", new[] { "options", "choices" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            DropDownListSetting listSetting = componentType.Component as DropDownListSetting;
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
