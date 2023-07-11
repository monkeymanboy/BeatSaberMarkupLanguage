namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLStringValue : BSMLValue
    {
        private string value;

        public BSMLStringValue(string value, string memberName = "")
        {
            this.value = value;
            MemberName = memberName;
        }

        public override string MemberName { get; }

        public override object GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return $"{nameof(BSMLStringValue)}[{nameof(MemberName)}={MemberName}, {nameof(FromUIValue)}={FromUIValue}, {nameof(value)}={value}]";
        }

        public override void SetValue(object value)
        {
            this.value = value.ToString();
        }
    }
}
