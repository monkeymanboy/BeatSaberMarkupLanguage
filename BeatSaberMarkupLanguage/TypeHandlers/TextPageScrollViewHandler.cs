using System;
using System.Collections.Generic;
using HMUI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextPageScrollView))]
    public class TextPageScrollViewHandler : TypeHandler<TextPageScrollView>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "text", new[] { "text" } },
        };

        public override Dictionary<string, Action<TextPageScrollView, string>> Setters => new()
        {
            { "text", new Action<TextPageScrollView, string>((component, value) => component.SetText(value ?? string.Empty)) },
        };
    }
}
