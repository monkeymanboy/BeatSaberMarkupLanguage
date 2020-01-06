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
    [ComponentHandler(typeof(Image))]
    class ImageHandler : TypeHandler<Image>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[]{"source", "src"} },
            { "preserveAspect", new[]{"preserve-aspect"} }
        };

        public override Dictionary<string, Action<Image, string>> Setters => new Dictionary<string, Action<Image, string>>()
        {
            { "image", (image, imagePath) => {  SetImage(image, imagePath); } },
            { "preserveAspect", (image, preserveAspect) => { image.preserveAspect = bool.Parse(preserveAspect); } }

        };

        public override void HandleType(ComponentTypeWithData obj, BSMLParserParams parserParams)
        {
            Image image = obj.component as Image;
            if (image != null)
            {
                if (obj.data.TryGetValue("image", out string imagePath))
                {
                    SetImage(image, imagePath);
                }

                if (obj.data.TryGetValue("preserveAspect", out string preserveAspect))
                {
                    image.preserveAspect = bool.Parse(preserveAspect);
                }
            }
        }

        public void SetImage(Image image, string imagePath)
        {
            if (imagePath.StartsWith("#"))
            {
                var imgName = imagePath.Substring(1);
                image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == imgName);
            }
            else
            {
                image.sprite = Utilities.FindSpriteInAssembly(imagePath);
            }

        }
    }
}
