using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Graphic))]
    internal class GraphicHandler : TypeHandler<Graphic>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "raycastPadding", new[] { "raycast-padding" } },
            { "raycastTarget", new[] { "raycast-target" } },
        };

        public override Dictionary<string, Action<Graphic, string>> Setters => new()
        {
            { "raycastPadding", new Action<Graphic, string>((graphic, value) => graphic.raycastPadding = Parse.Vector4(value)) },
            { "raycastTarget", new Action<Graphic, string>((graphic, value) => graphic.raycastTarget = Parse.Bool(value)) },
        };
    }
}
