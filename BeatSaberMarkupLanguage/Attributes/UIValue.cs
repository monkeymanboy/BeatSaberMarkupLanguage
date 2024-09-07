using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIValue(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}
