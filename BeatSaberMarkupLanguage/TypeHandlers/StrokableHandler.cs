using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using static BeatSaberMarkupLanguage.Components.Strokable;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Strokable))]
    public class StrokableHandler : TypeHandler<Strokable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "strokeColor", new[] { "stroke-color" } },
            { "strokeType", new[] { "stroke-type" } },
        };

        public override Dictionary<string, Action<Strokable, string>> Setters => new Dictionary<string, Action<Strokable, string>>()
        {
            { "strokeColor", new Action<Strokable, string>(SetColor) },
            { "strokeType", new Action<Strokable, string>(SetType) },
        };

        public static void SetColor(Strokable strokable, string strokeColor)
        {
            strokable.SetColor(strokeColor);
        }

        public static void SetType(Strokable strokable, string strokeType)
        {
            strokable.SetType((StrokeType)Enum.Parse(typeof(StrokeType), strokeType));
        }
    }
}
