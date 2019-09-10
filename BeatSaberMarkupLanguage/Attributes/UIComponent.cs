using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    public class UIComponent : Attribute
    {
        public string id;

        public UIComponent(string id)
        {
            this.id = id;
        }
    }
}
