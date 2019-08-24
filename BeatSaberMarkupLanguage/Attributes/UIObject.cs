using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
