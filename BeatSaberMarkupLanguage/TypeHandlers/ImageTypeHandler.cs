using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    
    [ComponentHandler(typeof(RawImage))]
    public class ImageTypeHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "width", new[]{"width"} },
            { "height", new[]{"height"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            RawImage image = obj as RawImage;

            //var element = image.gameObject.AddComponent<AspectRatioFitter>();
            //element.aspectRatio = 1f;
            //element.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            var layout = image.gameObject.AddComponent<LayoutElement>();

            if (data.ContainsKey("width"))
            {
                layout.minWidth = float.Parse(data["width"]);
            }
            if (data.ContainsKey("height"))
            {
                layout.minHeight = float.Parse(data["height"]);
            }
        }
    }
}
