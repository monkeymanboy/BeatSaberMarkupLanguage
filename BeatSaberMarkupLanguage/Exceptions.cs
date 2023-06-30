using System;
using System.Reflection;
using UnityEngine.Networking;

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

    public class UnityWebRequestException : Exception
    {
        public UnityWebRequestException(string message, UnityWebRequest webRequest)
            : base(message)
        {
            this.WebRequest = webRequest;
        }

        public override string Message => $"{base.Message}\nRequest URL: {WebRequest.url}\nResponse Code: {WebRequest.responseCode}\nError: {WebRequest.error}";

        public UnityWebRequest WebRequest { get; }
    }
}
