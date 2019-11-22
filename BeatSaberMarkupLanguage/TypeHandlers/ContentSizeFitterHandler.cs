using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ContentSizeFitter))]
    public class ContentSizeFitterHandler : TypeHandler<ContentSizeFitter>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "horizontalFit", new[]{ "horizontal-fit" } },
            { "verticalFit", new[]{ "vertical-fit"} }
        };

        public override Dictionary<string, Action<ContentSizeFitter, string>> Setters => _setters;
        private Dictionary<string, Action<ContentSizeFitter, string>> _setters = new Dictionary<string, Action<ContentSizeFitter, string>>()
        {
            {"horizontalFit", new Action<ContentSizeFitter, string>((component, value) => component.horizontalFit = (FitMode)Enum.Parse(typeof(FitMode), value)) },
            {"verticalFit", new Action<ContentSizeFitter, string>((component, value) => component.verticalFit = (FitMode)Enum.Parse(typeof(FitMode), value)) }
        };
    }
}
