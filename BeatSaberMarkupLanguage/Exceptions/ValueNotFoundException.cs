using BeatSaberMarkupLanguage.Util;

namespace BeatSaberMarkupLanguage
{
    public class ValueNotFoundException : BSMLException
    {
        internal ValueNotFoundException(string valueName, object host)
            : base($"Value '{valueName}' not found on object of type {host.GetTypeFullNameSafe()}")
        {
            this.ValueName = valueName;
            this.Host = host;
        }

        public string ValueName { get; }

        public object Host { get; }
    }
}
