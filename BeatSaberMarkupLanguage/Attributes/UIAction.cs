using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIAction : Attribute
    {
        public string id;

        public UIAction(string id)
        {
            this.id = id;
        }
    }
}
