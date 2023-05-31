using System;
using System.Reflection;

namespace BeatSaberMarkupLanguage
{
    public class BSMLException : Exception
    {
        public BSMLException()
        {
        }

        public BSMLException(string message)
            : base(message)
        {
        }

        public BSMLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class BSMLResourceException : BSMLException
    {
        public BSMLResourceException()
        {
        }

        public BSMLResourceException(Assembly assembly, string resourcePath)
            : base($"Error loading resource from assembly '{assembly?.GetName().Name ?? "N/A"}' ({resourcePath ?? "<NULL>"})")
        {
            Assembly = assembly;
            ResourcePath = resourcePath;
        }

        public BSMLResourceException(Assembly assembly, string resourcePath, Exception innerException)
           : base($"Error loading resource from assembly '{assembly?.GetName().Name ?? "N/A"}' ({resourcePath ?? "<NULL>"})", innerException)
        {
            Assembly = assembly;
            ResourcePath = resourcePath;
        }

        public BSMLResourceException(string message, Assembly assembly, string resourcePath)
            : base(message)
        {
            Assembly = assembly;
            ResourcePath = resourcePath;
        }

        public BSMLResourceException(string message, Assembly assembly, string resourcePath, Exception innerException)
            : base(message, innerException)
        {
            Assembly = assembly;
            ResourcePath = resourcePath;
        }

        public Assembly Assembly { get; }

        public string ResourcePath { get; }
    }
}
