using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Tab))]
    public class TabHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "tabName", new[]{"tab-name"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            Tab tab = (obj as Tab);
            if (!data.TryGetValue("tabName", out string tabName))
                throw new Exception("Tab must have a tab-name");
            tab.tabName = tabName;
        }
    }
}
