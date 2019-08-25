using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLParserParams
    {
        public object host;
        public Dictionary<string, BSMLAction> actions = new Dictionary<string, BSMLAction>();
        public Dictionary<string, BSMLValue> values = new Dictionary<string, BSMLValue>();
    }
}
