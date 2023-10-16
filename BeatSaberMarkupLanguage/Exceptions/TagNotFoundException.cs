namespace BeatSaberMarkupLanguage
{
    public class TagNotFoundException : BSMLException
    {
        internal TagNotFoundException(string tagName)
            : base($"Tag '{tagName}' not found")
        {
            this.TagName = tagName;
        }

        public string TagName { get; }
    }
}
