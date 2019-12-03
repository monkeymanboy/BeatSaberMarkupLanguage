using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler<Backgroundable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "background", new[]{ "bg", "background" } },
            { "backgroundColor", new[]{ "bg-color", "background-color" } }
        };
        public override Dictionary<string, Action<Backgroundable, string>> Setters => new Dictionary<string, Action<Backgroundable, string>>()
        {
            {"background", new Action<Backgroundable, string>((component, value) => component.ApplyBackground(value)) },
            {"backgroundColor", new Action<Backgroundable, string>(TrySetBackgroundColor) }
        };

        public static void TrySetBackgroundColor(Backgroundable background, string colorStr)
        {
            if (colorStr == "none")
                return;
            ColorUtility.TryParseHtmlString(colorStr, out Color color);
            background.background.color = color;
        }
    }
}
