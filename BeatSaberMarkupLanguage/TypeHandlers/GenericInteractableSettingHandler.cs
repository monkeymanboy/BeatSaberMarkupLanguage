using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(GenericInteractableSetting))]
    public class GenericInteractableSettingHandler : TypeHandler<GenericInteractableSetting>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "interactable", new[]{ "interactable" } }
        };

        public override Dictionary<string, Action<GenericInteractableSetting, string>> Setters => new Dictionary<string, Action<GenericInteractableSetting, string>>()
        {
            {"interactable", new Action<GenericInteractableSetting, string>(SetInteractable) }
        };

        public static void SetInteractable(GenericInteractableSetting interactable, string flag)
        {
            interactable.interactable = Parse.Bool(flag);
        }

    }
}