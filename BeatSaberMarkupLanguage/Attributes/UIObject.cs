using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIObject : Attribute
    {
        public string id;

        public UIObject(string id)
        {
            this.id = id;
        }
    }
}
