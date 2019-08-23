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
        /*
        public UnityEngine.TextAnchor childAlignment { get; set; }
        public RectOffset padding { get; set; }*/
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "padTop", new[]{"pad-top"} },
            { "padBottom", new[]{ "pad-bottom" } },
            { "padLeft", new[]{ "pad-left" } },
            { "padRight", new[]{ "pad-right" } }
        };
        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, Action> actions)
        {
            LayoutGroup layoutGroup = (obj as LayoutGroup);
            layoutGroup.padding = new RectOffset(data.ContainsKey("padLeft") ? int.Parse(data["padLeft"]) : layoutGroup.padding.left, data.ContainsKey("padRight") ? int.Parse(data["padRight"]) : layoutGroup.padding.right, data.ContainsKey("padTop") ? int.Parse(data["padTop"]) : layoutGroup.padding.top, data.ContainsKey("padBottom") ? int.Parse(data["padBottom"]) : layoutGroup.padding.bottom);
        }
    }
}
