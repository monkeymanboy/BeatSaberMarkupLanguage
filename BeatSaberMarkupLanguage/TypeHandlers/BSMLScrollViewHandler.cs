using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(BSMLScrollView))]
    public class BSMLScrollViewHandler : TypeHandler<BSMLScrollView>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "reserveButtonSpace", new[]{ "reserve-button-space" } }
        };
        public override Dictionary<string, Action<BSMLScrollView, string>> Setters => new Dictionary<string, Action<BSMLScrollView, string>>()
        {
            {"reserveButtonSpace", new Action<BSMLScrollView, string>((component, value) => component.ReserveButtonSpace = Parse.Bool(value)) }
        };
    }
}
