using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Tab))]
    public class TabHandler : TypeHandler<Tab>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "tabName", new[]{"tab-name"} }
        };

        public override Dictionary<string, Action<Tab, string>> Setters => _setters;
        private Dictionary<string, Action<Tab, string>> _setters = new Dictionary<string, Action<Tab, string>>()
        {
            {"tabName", new Action<Tab, string>((component, value) => component.tabName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException("tabName cannot be null or empty for Tab")) }
        };
    }
}
