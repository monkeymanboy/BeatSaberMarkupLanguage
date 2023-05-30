namespace BeatSaberMarkupLanguage.Parser
{
    public abstract class BSMLValue
    {
        public abstract string MemberName { get; }

        public bool FromUIValue { get; internal set; }

        public abstract void SetValue(object value);

        public abstract object GetValue();
    }
}
