using System.Reflection;

namespace BeatSaberMarkupLanguage
{
    public class ResourceNotFoundException : BSMLException
    {
        internal ResourceNotFoundException(Assembly assembly, string resourceName)
            : base($"No embedded resource named '{resourceName}' found in assembly '{assembly.FullName}'")
        {
            Assembly = assembly;
            ResourceName = resourceName;
        }

        public Assembly Assembly { get; }

        public string ResourceName { get; }
    }
}
