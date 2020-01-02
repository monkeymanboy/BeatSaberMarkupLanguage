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
    [ComponentHandler(typeof(ImageController))]
    class ImageHandler : TypeHandler<ImageController>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[]{"image", "source", "src", "img"} },
            { "useRawImage", new[]{"raw", "use-raw-image"} },
            {  "preserveAspect", new[]{"preserve-aspect"} }
        };

        public override Dictionary<string, Action<ImageController, string>> Setters => new Dictionary<string, Action<ImageController, string>>()
        {
            { "image", (image, iconPath) => { image.SetImage(iconPath); } },
            { "preserveAspect", (image, preserveAspect) => { image.SetPreserveAspect(bool.Parse(preserveAspect)); } }
        };

        public override void HandleType(ComponentTypeWithData obj, BSMLParserParams parserParams)
        {
            ImageController image = obj.component as ImageController;
            if (image != null)
            {
                if (obj.data.TryGetValue("useRawImage", out string useRawImage))
                {
                    image.SpawnImageComponent(bool.Parse(useRawImage));
                }
                else
                {
                    image.SpawnImageComponent(true);
                }

                if (obj.data.TryGetValue("image", out string iconPath))
                {
                    image.SetImage(iconPath);
                }

                if (obj.data.TryGetValue("preserveAspect", out string preserveAspect))
                {
                    image.SetPreserveAspect(bool.Parse(preserveAspect));
                }
            }
        }
    }
}
