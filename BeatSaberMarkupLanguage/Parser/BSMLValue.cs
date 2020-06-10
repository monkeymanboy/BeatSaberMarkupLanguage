namespace BeatSaberMarkupLanguage.Parser
{
    public abstract class BSMLValue
    {
        protected object host;
        public abstract string MemberName { get; }
        public bool FromUIValue { get; internal set; }
        public abstract void SetValue(object value);
        public abstract object GetValue();
    }
}
