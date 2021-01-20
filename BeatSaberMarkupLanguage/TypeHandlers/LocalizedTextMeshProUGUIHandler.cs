using Polyglot;
using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LocalizedTextMeshProUGUI))]
    public class LocalizedTextMeshProUGUIHandler : TypeHandler<LocalizedTextMeshProUGUI>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "textKey", new[] { "text-key" } },
            { "maintainTextAlignment", new[] { "maintain-text-alignment" } }
        };

        public override Dictionary<string, Action<LocalizedTextMeshProUGUI, string>> Setters => new Dictionary<string, Action<LocalizedTextMeshProUGUI, string>>()
        {
            {"textKey", new Action<LocalizedTextMeshProUGUI, string>((localizedText, value) => { localizedText.Key = value; localizedText.enabled = true; }) },
            {"maintainTextAlignment", new Action<LocalizedTextMeshProUGUI, string>((localizedText, value) => localizedText.MaintainTextAlignment = Parse.Bool(value)) }
        };
    }
}
