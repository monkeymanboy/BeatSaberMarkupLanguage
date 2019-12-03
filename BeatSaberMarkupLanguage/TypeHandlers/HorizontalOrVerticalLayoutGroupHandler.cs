using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(HorizontalOrVerticalLayoutGroup))]
    public class HorizontalOrVerticalLayoutGroupHandler : TypeHandler<HorizontalOrVerticalLayoutGroup>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "spacing", new[]{"spacing"} },
            { "childForceExpandWidth", new[]{ "child-expand-width" } },
            { "childForceExpandHeight", new[]{ "child-expand-height" } },
            { "childControlWidth", new[]{ "child-control-width" } },
            { "childControlHeight", new[]{ "child-control-height" } }
        };

        public override Dictionary<string, Action<HorizontalOrVerticalLayoutGroup, string>> Setters => new Dictionary<string, Action<HorizontalOrVerticalLayoutGroup, string>>()
        {
            {"spacing", new Action<HorizontalOrVerticalLayoutGroup, string>((component, value) => component.spacing = Parse.Float(value))},
            {"childForceExpandWidth", new Action<HorizontalOrVerticalLayoutGroup, string>((component, value) => component.childForceExpandWidth = Parse.Bool(value))},
            {"childForceExpandHeight", new Action<HorizontalOrVerticalLayoutGroup, string>((component, value) => component.childForceExpandHeight = Parse.Bool(value))},
            {"childControlWidth", new Action<HorizontalOrVerticalLayoutGroup, string>((component, value) => component.childControlWidth = Parse.Bool(value))},
            {"childControlHeight", new Action<HorizontalOrVerticalLayoutGroup, string>((component, value) => component.childControlHeight = Parse.Bool(value))}
        };
    }
}
