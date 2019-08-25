using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
