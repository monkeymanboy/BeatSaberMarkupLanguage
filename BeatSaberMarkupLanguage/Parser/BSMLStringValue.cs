namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLStringValue : BSMLValue
    {
        private string value;

        public BSMLStringValue(string value)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            Logger.log.Warn("You should not be using a macro defined value in a way that SetValue is called");
            this.value = value.ToString();
        }
    }
}
