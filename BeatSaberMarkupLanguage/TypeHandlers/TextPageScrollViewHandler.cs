using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextPageScrollView))]
    public class TextPageScrollViewHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{ "text" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            TextPageScrollView scrollView = (obj as TextPageScrollView);
            if (data.TryGetValue("text", out string text))
                scrollView.SetText(text);
        }
    }
}
