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
            { "backgroundColor0", new[] { "bg-color0", "background-color0" } },
            { "backgroundColor1", new[] { "bg-color1", "background-color1" } },
            { "backgroundAlpha", new[] { "bg-alpha", "background-alpha" } },
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters => new()
        {
            { "background", new Action<Backgroundable, string>((component, value) => component.ApplyBackground(value)) },
            { "backgroundColor", new Action<Backgroundable, string>((component, value) => component.ApplyColor(ParseColor(value))) },
            { "backgroundColor0", new Action<Backgroundable, string>((component, value) => component.ApplyColor0(ParseColor(value))) },
            { "backgroundColor1", new Action<Backgroundable, string>((component, value) => component.ApplyColor1(ParseColor(value))) },
            { "backgroundAlpha", new Action<Backgroundable, string>((component, value) => component.ApplyAlpha(float.Parse(value))) },
        };

        public static Color ParseColor(string colorStr) => ColorUtility.TryParseHtmlString(colorStr, out Color color) ? color : Color.white;
    }
}
