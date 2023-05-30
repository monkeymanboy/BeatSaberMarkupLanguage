using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Image))]
    internal class ImageHandler : TypeHandler<Image>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[] { "source" , "src" } },
            { "preserveAspect", new[] { "preserve-aspect" } },
            { "imageColor", new[] { "image-color", "img-color" } }
        };

        public override Dictionary<string, Action<Image, string>> Setters => new Dictionary<string, Action<Image, string>>()
        {
            { "image", new Action<Image, string>((image, path) => image.SetImage(path)) },
            { "preserveAspect", new Action<Image, string>((image, preserveAspect) => image.preserveAspect = bool.Parse(preserveAspect)) },
            { "imageColor", new Action<Image, string>((image, color) => image.color = GetColor(color)) }

        };

        private static Color GetColor(string colorStr)
        {
            if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                return color;
            Logger.log?.Warn($"Color {colorStr}, is not a valid color.");
            return Color.white;
        }
    }
}
