using System;

namespace BeatSaberMarkupLanguage
{
    public class BSMLException : Exception
    {
        internal BSMLException()
        {
        }

        internal BSMLException(string message)
            : base(message)
        {
        }

        internal BSMLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
