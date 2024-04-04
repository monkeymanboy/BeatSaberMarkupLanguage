using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components;
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
                { "handleImage", (indic, src) => GetHandleImage(indic).SetImageAsync(src).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted) },
            };

        private static Image GetHandleImage(BSMLScrollIndicator indicator) => indicator.Handle.GetComponent<Image>();

        private static void TrySetHandleColor(BSMLScrollIndicator indicator, string colorString)
        {
            GetHandleImage(indicator).color = Parse.Color(colorString);
        }
    }
}
