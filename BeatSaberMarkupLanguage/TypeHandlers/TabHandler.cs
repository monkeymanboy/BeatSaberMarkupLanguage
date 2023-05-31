using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Tab))]
    public class TabHandler : TypeHandler<Tab>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "tabName", new[] { "tab-name" } },
            { "tabNameKey", new[] { "tab-name-key" } },
        };

        public override Dictionary<string, Action<Tab, string>> Setters => new()
        {
            { "tabName", new Action<Tab, string>((component, value) => component.TabName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException("tabName cannot be null or empty for Tab")) },
            { "tabNameKey", new Action<Tab, string>((component, value) => component.TabKey = value) },
        };
    }
}
