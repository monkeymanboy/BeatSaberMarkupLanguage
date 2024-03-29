﻿using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Selectable))]
    public class SelectableHandler : TypeHandler<Selectable>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "interactable", new[] { "interactable" } },
        };

        public override Dictionary<string, Action<Selectable, string>> Setters => new()
        {
            { "interactable", new Action<Selectable, string>(SetInteractable) },
        };

        public static void SetInteractable(Selectable selectable, string flag)
        {
            selectable.interactable = Parse.Bool(flag);
        }
    }
}
