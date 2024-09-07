using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIObject : Attribute
    {
        public string Id;

        public UIObject(string id)
        {
            this.Id = id;
        }
    }
}
