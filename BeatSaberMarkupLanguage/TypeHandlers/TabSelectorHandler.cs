using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TabSelector))]
    public class TabSelectorHandler : TypeHandler<TabSelector>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "tabTag", new[] { "tab-tag" } },
            { "pageCount", new[] { "page-count" } },
            { "leftButtonTag", new[] { "left-button-tag" } },
            { "rightButtonTag", new[] { "right-button-tag" } },
        };

        public override Dictionary<string, Action<TabSelector, string>> Setters => new()
        {
            { "pageCount", new Action<TabSelector, string>((component, value) => component.PageCount = Parse.Int(value)) },
            { "leftButtonTag", new Action<TabSelector, string>((component, value) => component.LeftButtonTag = value) },
            { "rightButtonTag", new Action<TabSelector, string>((component, value) => component.RightButtonTag = value) },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            TabSelector tabSelector = componentType.Component as TabSelector;
            tabSelector.ParserParams = parserParams;
            if (!componentType.Data.TryGetValue("tabTag", out string tabTag))
            {
                throw new MissingAttributeException(this, "tab-tag");
            }

            tabSelector.TabTag = tabTag;
            parserParams.AddEvent("post-parse", tabSelector.Setup);
        }
    }
}
