using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIAction : Attribute
    {
        public string Id;

        public UIAction(string id)
        {
            this.Id = id;
        }
    }
}
