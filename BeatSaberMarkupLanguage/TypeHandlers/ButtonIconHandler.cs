using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ButtonIconImage))]
    public class ButtonIconHandler : TypeHandler<ButtonIconImage>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "icon", new[] { "icon" } },
            { "iconSkew", new[] { "icon-skew" } },
            { "showUnderline", new[] { "show-underline" } },
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new()
        {
            { "icon", new Action<ButtonIconImage, string>((image, value) => image.SetIcon(value)) },
            { "iconSkew", new Action<ButtonIconImage, string>((image, value) => image.SetSkew(Parse.Float(value))) },
            { "showUnderline", new Action<ButtonIconImage, string>((image, value) => image.SetUnderlineActive(Parse.Bool(value))) },
        };
    }
}
