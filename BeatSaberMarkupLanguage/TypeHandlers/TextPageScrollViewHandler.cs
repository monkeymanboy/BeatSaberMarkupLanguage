using HMUI;
using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextPageScrollView))]
    public class TextPageScrollViewHandler : TypeHandler<TextPageScrollView>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{ "text" } }
        };

        public override Dictionary<string, Action<TextPageScrollView, string>> Setters => new Dictionary<string, Action<TextPageScrollView, string>>()
        {
            {"text", new Action<TextPageScrollView, string>((component, value) => component.SetText(value ?? string.Empty)) }
        };
    }
}
