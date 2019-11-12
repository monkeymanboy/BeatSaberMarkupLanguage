using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TabSelector))]
    public class TabSelectorHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "tabTag", new[]{"tab-tag"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            TabSelector tabSelector = (obj as TabSelector);
            tabSelector.parserParams = parserParams;
            if (!data.TryGetValue("tabTag", out string tabTag))
                throw new Exception("Tab Selector must have a tab-tag");
            tabSelector.tabTag = tabTag;
            parserParams.AddEvent("post-parse", tabSelector.Setup);
        }
    }
}
