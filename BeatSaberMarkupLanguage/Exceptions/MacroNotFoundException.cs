namespace BeatSaberMarkupLanguage
{
    public class MacroNotFoundException : BSMLException
    {
        internal MacroNotFoundException(string macroName)
            : base($"Macro '{macroName}' not found")
        {
            this.MacroName = macroName;
        }

        public string MacroName { get; }
    }
}
