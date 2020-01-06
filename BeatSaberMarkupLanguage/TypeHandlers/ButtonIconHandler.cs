using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ButtonIconImage))]
    public class ButtonIconHandler : TypeHandler<ButtonIconImage>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "icon", new[]{"icon"} }
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new Dictionary<string, Action<ButtonIconImage, string>>()
        {
            { "icon", (images, iconPath) => { images.SetIcon(iconPath); } }
        };

    }
}
