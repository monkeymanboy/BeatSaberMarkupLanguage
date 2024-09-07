using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIAction(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}
