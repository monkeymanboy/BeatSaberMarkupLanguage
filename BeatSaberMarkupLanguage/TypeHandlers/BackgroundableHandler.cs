using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using HMUI;

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
            { "backgroundGradient", new[] { "background-gradient" } },
            { "backgroundAlpha", new[] { "bg-alpha", "background-alpha" } },
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters => new()
        {
            { "background", new Action<Backgroundable, string>((component, value) => component.ApplyBackground(value)) },
            { "backgroundColor", new Action<Backgroundable, string>((component, value) => component.Background.color = Parse.Color(value, component.Background.color.a)) },
            {
                "backgroundColor0", new Action<Backgroundable, string>((component, value) =>
                {
                    if (component.Background is ImageView imageView)
                    {
                        imageView.color0 = Parse.Color(value, imageView.color0.a);
                    }
                })
            },
            {
                "backgroundColor1", new Action<Backgroundable, string>((component, value) =>
                {
                    if (component.Background is ImageView imageView)
                    {
                        imageView.color1 = Parse.Color(value, imageView.color1.a);
                    }
                })
            },
            {
                "backgroundGradient", new Action<Backgroundable, string>((component, value) =>
                {
                    if (component.Background is ImageView imageView)
                    {
                        imageView.gradient = Parse.Bool(value);
                    }
                })
            },
            { "backgroundAlpha", new Action<Backgroundable, string>((component, value) => component.ApplyAlpha(Parse.Float(value))) },
        };
    }
}
