using System;
using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLPropertyValue : BSMLValue
    {
        private readonly object host;
        internal PropertyInfo propertyInfo;
        public override string MemberName => propertyInfo?.Name;

        public BSMLPropertyValue(object host, PropertyInfo propertyInfo, bool fromUiValue = true)
        {
            this.host = host;
            this.propertyInfo = propertyInfo;
            FromUIValue = fromUiValue;
        }

        public override object GetValue()
        {
            return propertyInfo.GetGetMethod(true).Invoke(host, Array.Empty<object>());
        }

        public override void SetValue(object value)
        {
            propertyInfo.GetSetMethod(true).Invoke(host, new object[] { value });
        }
    }
}
