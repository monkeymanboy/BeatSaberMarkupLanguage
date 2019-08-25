using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Parser
{
    public abstract class BSMLValue
    {
        public abstract void SetValue(object value);
        public abstract object GetValue();
    }
}
