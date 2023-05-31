using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler<Backgroundable>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "background", new[] { "bg", "background" } },
            { "backgroundColor", new[] { "bg-color", "background-color" } },
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters => new()
        {
            { "background", new Action<Backgroundable, string>((component, value) => component.ApplyBackground(value)) },
            { "backgroundColor", new Action<Backgroundable, string>(TrySetBackgroundColor) },
        };

        public static void TrySetBackgroundColor(Backgroundable background, string colorStr)
        {
            if (colorStr == "none")
            {
                return;
            }

            ColorUtility.TryParseHtmlString(colorStr, out Color color);
            background.background.color = color;
        }
    }
}
