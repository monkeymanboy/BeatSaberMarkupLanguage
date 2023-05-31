using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Glowable))]
    public class GlowableHandler : TypeHandler<Glowable>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "glowColor", new[] { "glow-color" } },
        };

        public override Dictionary<string, Action<Glowable, string>> Setters => new()
        {
            { "glowColor", new Action<Glowable, string>(SetGlow) },
        };

        public static void SetGlow(Glowable glowable, string glowColor)
        {
            glowable.SetGlow(glowColor);
        }
    }
}
