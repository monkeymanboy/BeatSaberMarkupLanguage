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
            fieldInfo.SetValue(host, value);
        }
    }
}
