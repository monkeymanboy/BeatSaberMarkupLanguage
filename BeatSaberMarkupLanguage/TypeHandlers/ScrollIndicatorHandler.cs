using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(BSMLScrollIndicator))]
    public class ScrollIndicatorHandler : TypeHandler<BSMLScrollIndicator>
    {
        public override Dictionary<string, string[]> Props { get; }
            = new Dictionary<string, string[]>
            {
                { "handleColor", new[] { "handle-color" } },
                { "handleImage", new[] { "handle-image" } },
            };

        public override Dictionary<string, Action<BSMLScrollIndicator, string>> Setters { get; }
            = new Dictionary<string, Action<BSMLScrollIndicator, string>>
            {
                { "handleColor", TrySetHandleColor },
                { "handleImage", (indic, src) => GetHandleImage(indic).SetImage(src) },
            };

        private static Image GetHandleImage(BSMLScrollIndicator indicator) => indicator.Handle.GetComponent<Image>();

        private static void TrySetHandleColor(BSMLScrollIndicator indicator, string colorString)
        {
            if (!ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                Logger.Log.Warn($"String {colorString} not a valid color");
                return;
            }

            GetHandleImage(indicator).color = color;
        }
    }
}
