using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
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
            { "image", new Action<RawImage, string>(SetImage) }
        };

        public void SetImage(RawImage image, string imagePath)
        {
            if (imagePath.StartsWith("#"))
            {
                string imgName = imagePath.Substring(1);
                try
                {
                    image.texture = Resources.FindObjectsOfTypeAll<Texture>().First(x => x.name == imgName);
                }
                catch
                {
                    Logger.log.Error($"Could not find Texture with image name {imgName}");
                }
            }
            else
            {
                image.texture = Utilities.FindTextureInAssembly(imagePath);
            }

        }
    }
}
