using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIComponent : Attribute
    {
        public string Id;

        public UIComponent(string id)
        {
            this.Id = id;
        }
    }
}
