using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
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
            { "image", new Action<Image, string>(SetImage) },
            { "preserveAspect", new Action<Image, string>((image, preserveAspect) => image.preserveAspect = bool.Parse(preserveAspect)) }

        };

        public void SetImage(Image image, string imagePath)
        {
            if (imagePath.StartsWith("#"))
            {
                string imgName = imagePath.Substring(1);
                try
                {
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == imgName);
                }
                catch
                {
                    Logger.log.Error($"Could not find Texture with image name {imgName}");
                }
            }
            else
            {
                image.sprite = Utilities.FindSpriteInAssembly(imagePath);
            }

        }
    }
}
