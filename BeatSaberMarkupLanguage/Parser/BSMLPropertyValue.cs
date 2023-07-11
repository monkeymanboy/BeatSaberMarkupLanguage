using System;
using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLPropertyValue : BSMLValue
    {
        private readonly object host;

        public BSMLPropertyValue(object host, PropertyInfo propertyInfo, bool fromUiValue = true)
        {
            this.host = host;
            this.PropertyInfo = propertyInfo;
            FromUIValue = fromUiValue;
        }

        public override string MemberName => PropertyInfo?.Name;

        internal PropertyInfo PropertyInfo { get; set; }

        public override object GetValue()
        {
            MethodInfo getMethod = PropertyInfo.GetGetMethod(true);

            if (getMethod == null)
            {
                Logger.Log.Error($"Trying to get value of property '{PropertyInfo.Name}' on type '{PropertyInfo.DeclaringType.FullName}' but it does not have a getter.");
                return null;
            }

            return getMethod.Invoke(host, Array.Empty<object>());
        }

        public override string ToString()
        {
            return $"{nameof(BSMLValue)}[{nameof(MemberName)}={MemberName}, {nameof(FromUIValue)}={FromUIValue}, {nameof(host)}={host}, {nameof(PropertyInfo)}={PropertyInfo}]";
        }

        public override void SetValue(object value)
        {
            MethodInfo setMethod = PropertyInfo.GetSetMethod(true);

            if (setMethod == null)
            {
                Logger.Log.Error($"Trying to set value of property '{PropertyInfo.Name}' on type '{PropertyInfo.DeclaringType.FullName}' but it does not have a setter.");
                return;
            }

            setMethod.Invoke(host, new object[] { value });
        }
    }
}
