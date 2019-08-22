using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Attributes
{
    class UIComponent : Attribute
    {
        public string id;
        public UIComponent(string id)
        {
            this.id = id;
        }
    }
}
