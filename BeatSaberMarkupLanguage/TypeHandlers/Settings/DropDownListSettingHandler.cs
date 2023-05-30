using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(DropDownListSetting))]
    public class DropDownListSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "options", new[] { "options", "choices" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            DropDownListSetting listSetting = componentType.component as DropDownListSetting;
            if (componentType.data.TryGetValue("options", out string options))
            {
                if (!parserParams.values.TryGetValue(options, out BSMLValue values))
                {
                    throw new Exception($"Options '{options}' not found");
                }

                listSetting.values = values.GetValue() as List<object>;
            }
            else
            {
                throw new Exception("List must have associated options");
            }
        }
    }
}
