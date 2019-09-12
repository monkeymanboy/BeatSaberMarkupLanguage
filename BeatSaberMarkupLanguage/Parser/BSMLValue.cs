namespace BeatSaberMarkupLanguage.Parser
{
    public abstract class BSMLValue
    {
        public abstract void SetValue(object value);
        public abstract object GetValue();
    }
}
