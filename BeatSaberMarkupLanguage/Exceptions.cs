using System;

namespace BeatSaberMarkupLanguage
{
    public class BSMLException : Exception
    {
        public BSMLException()
        {
        }

        public BSMLException(string message) : base(message)
        {
        }

        public BSMLException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
