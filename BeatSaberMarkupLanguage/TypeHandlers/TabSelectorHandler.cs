using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TabSelector))]
    public class TabSelectorHandler : TypeHandler<TabSelector>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "tabTag", new[]{"tab-tag"} },
            {"hasSeparator", new[]{"has-separator"} },
            {"pageCount", new[]{"page-count"} },
            {"leftButtonTag", new[]{"left-button-tag"} },
            {"rightButtonTag", new[]{"right-button-tag"} }
        };

        public override Dictionary<string, Action<TabSelector, string>> Setters => new Dictionary<string, Action<TabSelector, string>>()
        {
            {"hasSeparator", new Action<TabSelector,string>(SetSeparator) },
            {"pageCount", new Action<TabSelector,string>((component, value) => component.PageCount = Parse.Int(value)) },
            {"leftButtonTag", new Action<TabSelector,string>((component, value) => component.leftButtonTag = value) },
            {"rightButtonTag", new Action<TabSelector,string>((component, value) => component.rightButtonTag = value) }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            TabSelector tabSelector = (componentType.component as TabSelector);
            tabSelector.parserParams = parserParams;
            if (!componentType.data.TryGetValue("tabTag", out string tabTag))
                throw new Exception("Tab Selector must have a tab-tag");
            tabSelector.tabTag = tabTag;
            parserParams.AddEvent("post-parse", tabSelector.Setup);
        }

        private void SetSeparator(TabSelector tabSelector, string hasSeparator)
        {
            tabSelector.textSegmentedControl.SetField<SegmentedControl, Transform>("_separatorPrefab", Parse.Bool(hasSeparator) ? (Resources.FindObjectsOfTypeAll<TextSegmentedControl>().First(x => x.GetField<Transform, SegmentedControl > ("_separatorPrefab") != null).GetField<Transform, SegmentedControl>("_separatorPrefab")) : null);
        }
    }
}
