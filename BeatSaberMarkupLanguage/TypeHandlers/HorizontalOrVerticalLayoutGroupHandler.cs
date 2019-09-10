using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(HorizontalOrVerticalLayoutGroup))]
    public class HorizontalOrVerticalLayoutGroupHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "spacing", new[]{"spacing"} },
            { "childForceExpandWidth", new[]{ "child-expand-width" } },
            { "childForceExpandHeight", new[]{ "child-expand-height" } },
            { "childControlWidth", new[]{ "child-control-width" } },
            { "childControlHeight", new[]{ "child-control-height" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            HorizontalOrVerticalLayoutGroup layoutGroup = (obj as HorizontalOrVerticalLayoutGroup);
            foreach (KeyValuePair<string, string> pair in data)
            {
                if (pair.Key == "spacing")
                {
                    layoutGroup.SetProperty(pair.Key, Parse.Float(pair.Value));
                }
                else
                {
                    layoutGroup.SetProperty(pair.Key, Parse.Bool(pair.Value));
                }
            }
        }
    }
}
