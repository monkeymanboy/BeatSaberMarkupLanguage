using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMultiplayer.UI.UIElements
{
    [ComponentHandler(typeof(RawImage))]
    class ImageHandler : TypeHandler<RawImage>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[]{"image", "source", "src", "img"} }
        };

        public override Dictionary<string, Action<RawImage, string>> Setters => new Dictionary<string, Action<RawImage, string>>()
        {
            { "image", (image, iconPath) => { image.texture = Utilities.FindTextureInAssembly(iconPath); } }
        };

        public override void HandleType(ComponentTypeWithData obj, BSMLParserParams parserParams)
        {
            RawImage image = obj.component as RawImage;
            if (image != null)
            {
                if (obj.data.TryGetValue("image", out string iconPath))
                {
                    image.texture = Utilities.FindTextureInAssembly(iconPath);
                }
            }
        }
    }
}
