using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIValue : Attribute
    {
        public string Id;

        public UIValue(string id)
        {
            this.Id = id;
        }
    }
}
