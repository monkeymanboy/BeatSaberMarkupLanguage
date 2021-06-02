using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ScrollingText))]
    public class ScrollingTextHandler : TypeHandler<ScrollingText>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "movementType", new[]{ "movement-type" } },
            { "animationType", new[]{ "animation-type" } },
            { "textWidthRatioThreshold", new[]{ "text-width-ratio-threshold" } },
            { "pauseDuration", new[]{ "pause-duration" } },
            { "scrollDuration", new[]{ "scroll-duration" } },
            { "scrollSpeed", new[]{ "scroll-speed" } },
            { "alwaysScroll", new[]{ "always-scroll" } },
        };

        public override Dictionary<string, Action<ScrollingText, string>> Setters => new Dictionary<string, Action<ScrollingText, string>>()
        {
            { "movementType", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.movementType = (ScrollingText.ScrollMovementType)Enum.Parse(typeof(ScrollingText.ScrollMovementType), value)) },
            { "animationType", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.animationType = (ScrollingText.ScrollAnimationType)Enum.Parse(typeof(ScrollingText.ScrollAnimationType), value)) },
            { "textWidthRatioThreshold", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.textWidthRatioThreshold = Parse.Float(value)) },
            { "pauseDuration", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.pauseDuration = Parse.Float(value)) },
            { "scrollDuration", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.scrollDuration = Parse.Float(value)) },
            { "scrollSpeed", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.scrollSpeed = Parse.Float(value)) },
            { "alwaysScroll", new Action<ScrollingText, string>((scrollingText, value) => scrollingText.alwaysScroll = Parse.Bool(value)) },
        };
    }
}
