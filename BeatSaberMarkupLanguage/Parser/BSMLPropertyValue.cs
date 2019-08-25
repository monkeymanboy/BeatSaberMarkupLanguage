using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLPropertyValue : BSMLValue
    {
        private object host;
        private PropertyInfo propertyInfo;
        public BSMLPropertyValue(object host, PropertyInfo propertyInfo)
        {
            this.host = host;
            this.propertyInfo = propertyInfo;
        }

        public override object GetValue()
        {
            return propertyInfo.GetGetMethod(false).Invoke(host, new object[] { });
        }

        public override void SetValue(object value)
        {
            propertyInfo.GetSetMethod(false).Invoke(host, new object[] { value });
        }
    }
}
