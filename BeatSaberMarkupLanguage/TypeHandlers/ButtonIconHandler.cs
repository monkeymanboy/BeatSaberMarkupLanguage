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
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new()
        {
            { "icon", new Action<ButtonIconImage, string>((images, iconPath) => images.SetIcon(iconPath)) },
        };
    }
}
