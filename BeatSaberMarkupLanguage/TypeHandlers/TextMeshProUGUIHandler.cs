using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUIHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "fontSize", new[]{"font-size"} },
            { "alignment", new[]{"align"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            TextMeshProUGUI textMesh = obj as TextMeshProUGUI;
            if (data.ContainsKey("text"))
                textMesh.text = data["text"];
            if (data.ContainsKey("fontSize"))
                textMesh.fontSize = float.Parse(data["fontSize"]);
            if(data.ContainsKey("alignment"))
            textMesh.alignment = (TextAlignmentOptions) Enum.Parse(typeof(TextAlignmentOptions), data["alignment"]);
        }
    }
}
