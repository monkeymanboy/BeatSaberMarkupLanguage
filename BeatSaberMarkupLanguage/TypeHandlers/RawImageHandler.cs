using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
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
    class RawImageHandler : TypeHandler<RawImage>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[]{"source", "src"} }
        };

        public override Dictionary<string, Action<RawImage, string>> Setters => new Dictionary<string, Action<RawImage, string>>()
        {
            { "image", (image, imagePath) => {  SetImage(image, imagePath); } }
        };

        public override void HandleType(ComponentTypeWithData obj, BSMLParserParams parserParams)
        {
            RawImage image = obj.component as RawImage;
            if (image != null)
            {
                if (obj.data.TryGetValue("image", out string imagePath))
                {
                    SetImage(image, imagePath);
                }
            }
        }

        public void SetImage(RawImage image, string imagePath)
        {
            if (imagePath.StartsWith("#"))
            {
                var imgName = imagePath.Substring(1);
                image.texture = Resources.FindObjectsOfTypeAll<Texture>().First(x => x.name == imgName);
            }
            else
            {
                image.texture = Utilities.FindTextureInAssembly(imagePath);
            }

        }
    }
}
