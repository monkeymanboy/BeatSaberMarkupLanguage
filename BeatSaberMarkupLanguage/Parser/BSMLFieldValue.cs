using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLFieldValue : BSMLValue
    {
        private readonly object host;

        public BSMLFieldValue(object host, FieldInfo fieldInfo, bool fromUiValue = true)
        {
            this.host = host;
            this.FieldInfo = fieldInfo;
            FromUIValue = fromUiValue;
        }

        public override string MemberName => FieldInfo?.Name;

        internal FieldInfo FieldInfo { get; set; }

        public override object GetValue()
        {
            return FieldInfo.GetValue(host);
        }

        public override void SetValue(object value)
        {
            if (FieldInfo.Attributes.HasFlag(FieldAttributes.InitOnly))
            {
                Logger.Log.Warn($"Trying to set value of field '{FieldInfo.Name}' on type '{FieldInfo.DeclaringType.FullName}' which is marked as read-only. This is unsupported behavior and may be removed in a future release.");
            }

            FieldInfo.SetValue(host, value);
        }
    }
}
