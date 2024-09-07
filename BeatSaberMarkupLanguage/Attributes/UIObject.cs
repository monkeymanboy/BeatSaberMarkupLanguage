using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIObject(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}
