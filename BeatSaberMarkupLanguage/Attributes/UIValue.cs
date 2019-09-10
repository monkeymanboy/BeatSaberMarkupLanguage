using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIValue : Attribute
    {
        public string id;

        public UIValue(string id)
        {
            this.id = id;
        }
    }
}
