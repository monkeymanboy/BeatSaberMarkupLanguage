using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(GenericInteractableSetting))]
    public class GenericInteractableSettingHandler : TypeHandler<GenericInteractableSetting>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "interactable", new[] { "interactable" } },
        };

        public override Dictionary<string, Action<GenericInteractableSetting, string>> Setters => new()
        {
            { "interactable", new Action<GenericInteractableSetting, string>(SetInteractable) },
        };

        public static void SetInteractable(GenericInteractableSetting interactable, string flag)
        {
            interactable.Interactable = Parse.Bool(flag);
        }
    }
}
