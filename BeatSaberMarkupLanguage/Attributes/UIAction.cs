using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Attributes
{
    class UIAction : Attribute
    {
        public string id;
        public UIAction(string id)
        {
            this.id = id;
        }
    }
}
