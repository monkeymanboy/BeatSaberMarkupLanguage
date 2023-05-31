using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLFieldValue : BSMLValue
    {
        internal FieldInfo fieldInfo;
        private readonly object host;

        public BSMLFieldValue(object host, FieldInfo fieldInfo, bool fromUiValue = true)
        {
            this.host = host;
            this.fieldInfo = fieldInfo;
            FromUIValue = fromUiValue;
        }

        public override string MemberName => fieldInfo?.Name;

        public override object GetValue()
        {
            return fieldInfo.GetValue(host);
        }

        public override void SetValue(object value)
        {
            if (fieldInfo.Attributes.HasFlag(FieldAttributes.InitOnly))
            {
                Logger.Log.Warn($"Trying to set value of field '{fieldInfo.Name}' on type '{fieldInfo.DeclaringType.FullName}' which is marked as read-only. This is unsupported behavior and may be removed in a future release.");
            }

            fieldInfo.SetValue(host, value);
        }
    }
}
