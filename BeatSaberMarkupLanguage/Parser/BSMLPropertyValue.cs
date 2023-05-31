using System;
using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLPropertyValue : BSMLValue
    {
        internal PropertyInfo propertyInfo;
        private readonly object host;

        public BSMLPropertyValue(object host, PropertyInfo propertyInfo, bool fromUiValue = true)
        {
            this.host = host;
            this.propertyInfo = propertyInfo;
            FromUIValue = fromUiValue;
        }

        public override string MemberName => propertyInfo?.Name;

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
