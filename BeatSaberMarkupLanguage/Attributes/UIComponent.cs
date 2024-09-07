using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIComponent(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}
