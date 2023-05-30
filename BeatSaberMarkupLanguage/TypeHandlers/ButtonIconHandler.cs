using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ButtonIconImage))]
    public class ButtonIconHandler : TypeHandler<ButtonIconImage>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "icon", new[] { "icon" } },
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new Dictionary<string, Action<ButtonIconImage, string>>()
        {
            { "icon", new Action<ButtonIconImage, string>((images, iconPath) => images.SetIcon(iconPath)) },
        };
    }
}
