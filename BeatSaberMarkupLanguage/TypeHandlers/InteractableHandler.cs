using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Interactable))]
    public class InteractableHandler : TypeHandler<Interactable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "interactable", new[]{ "interactable" } }
        };

        public override Dictionary<string, Action<Interactable, string>> Setters => new Dictionary<string, Action<Interactable, string>>()
        {
            {"interactable", new Action<Interactable, string>(SetInteractable) }
        };

        public static void SetInteractable(Interactable interactable, string flag)
        {
            interactable.interactable = Parse.Bool(flag);
        }

    }
}