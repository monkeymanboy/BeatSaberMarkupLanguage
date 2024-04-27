using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

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
            { "backgroundColor", new Action<Backgroundable, string>((component, value) => component.ApplyColor(Parse.Color(value))) },
            { "backgroundColor0", new Action<Backgroundable, string>((component, value) => component.ApplyColor0(Parse.Color(value))) },
            { "backgroundColor1", new Action<Backgroundable, string>((component, value) => component.ApplyColor1(Parse.Color(value))) },
            { "backgroundAlpha", new Action<Backgroundable, string>((component, value) => component.ApplyAlpha(Parse.Float(value))) },
        };
    }
}
