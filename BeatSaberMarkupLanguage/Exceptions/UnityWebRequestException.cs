using System;
using UnityEngine.Networking;

namespace BeatSaberMarkupLanguage
{
    public class UnityWebRequestException : Exception
    {
        internal UnityWebRequestException(string message, UnityWebRequest webRequest)
            : base($"{message}\nRequest URL: {webRequest.url}\nResponse Code: {webRequest.responseCode}\nError: {webRequest.error}")
        {
            this.WebRequest = webRequest;
        }

        public UnityWebRequest WebRequest { get; }
    }
}
