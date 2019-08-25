using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LayoutGroup))]
    public class LayoutGroupHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "padTop", new[]{"pad-top"} },
            { "padBottom", new[]{ "pad-bottom" } },
            { "padLeft", new[]{ "pad-left" } },
            { "padRight", new[]{ "pad-right" } },
            { "pad", new[]{ "pad" } }
        };
        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            LayoutGroup layoutGroup = (obj as LayoutGroup);
            if (data.ContainsKey("pad"))
            {
                int padding = int.Parse(data["pad"]);
                layoutGroup.padding = new RectOffset(padding, padding, padding, padding);
            }
            layoutGroup.padding = new RectOffset(data.ContainsKey("padLeft") ? int.Parse(data["padLeft"]) : layoutGroup.padding.left, data.ContainsKey("padRight") ? int.Parse(data["padRight"]) : layoutGroup.padding.right, data.ContainsKey("padTop") ? int.Parse(data["padTop"]) : layoutGroup.padding.top, data.ContainsKey("padBottom") ? int.Parse(data["padBottom"]) : layoutGroup.padding.bottom);
        }
    }
}
