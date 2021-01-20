using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Tab))]
    public class TabHandler : TypeHandler<Tab>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "tabName", new[] { "tab-name" } },
            { "tabNameKey", new[] { "tab-name-key" } }
        };

        public override Dictionary<string, Action<Tab, string>> Setters => new Dictionary<string, Action<Tab, string>>()
        {
            {"tabName", new Action<Tab, string>((component, value) => component.TabName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException("tabName cannot be null or empty for Tab")) },
            {"tabNameKey", new Action<Tab, string>((component, value) => component.TabKey = value) }
        };
    }
}
