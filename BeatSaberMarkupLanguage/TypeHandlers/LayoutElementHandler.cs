using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LayoutElement))]
    public class LayoutElementHandler : TypeHandler<LayoutElement>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "ignoreLayout", new[] { "ignore-layout" } },
            { "preferredWidth", new[] { "preferred-width", "pref-width" } },
            { "preferredHeight", new[] { "preferred-height", "pref-height" } },
            { "minHeight", new[] { "min-height" } },
            { "minWidth", new[] { "min-width" } },
        };

        public override Dictionary<string, Action<LayoutElement, string>> Setters => new Dictionary<string, Action<LayoutElement, string>>()
        {
            { "ignoreLayout", new Action<LayoutElement, string>((layoutElement, value) => layoutElement.ignoreLayout = Parse.Bool(value)) },
            { "preferredWidth", new Action<LayoutElement, string>((layoutElement, value) => layoutElement.preferredWidth = Parse.Float(value)) },
            { "preferredHeight", new Action<LayoutElement, string>((layoutElement, value) => layoutElement.preferredHeight = Parse.Float(value)) },
            { "minHeight", new Action<LayoutElement, string>((layoutElement, value) => layoutElement.minHeight = Parse.Float(value)) },
            { "minWidth", new Action<LayoutElement, string>((layoutElement, value) => layoutElement.minWidth = Parse.Float(value)) },
        };
    }
}
