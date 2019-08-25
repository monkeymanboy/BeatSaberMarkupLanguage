using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLFieldValue : BSMLValue
    {
        private object host;
        private FieldInfo fieldInfo;
        public BSMLFieldValue(object host, FieldInfo fieldInfo)
        {
            this.host = host;
            this.fieldInfo = fieldInfo;
        }

        public override object GetValue()
        {
            return fieldInfo.GetValue(host);
        }

        public override void SetValue(object value)
        {
            fieldInfo.SetValue(host, value);
        }
    }
}
