using System;
using System.Collections.Generic;
using Polyglot;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LocalizedTextMeshProUGUI))]
    public class LocalizedTextMeshProUGUIHandler : TypeHandler<LocalizedTextMeshProUGUI>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "textKey", new[] { "text-key" } },
            { "maintainTextAlignment", new[] { "maintain-text-alignment" } },
        };

        public override Dictionary<string, Action<LocalizedTextMeshProUGUI, string>> Setters => new()
        {
            {
                "textKey", new Action<LocalizedTextMeshProUGUI, string>((localizedText, value) =>
                {
                    localizedText.Key = value;
                    localizedText.enabled = true;
                })
            },
            { "maintainTextAlignment", new Action<LocalizedTextMeshProUGUI, string>((localizedText, value) => localizedText.MaintainTextAlignment = Parse.Bool(value)) },
        };
    }
}
