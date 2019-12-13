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
            this.value = value.ToString();
        }
    }
}
