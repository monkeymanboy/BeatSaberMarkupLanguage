using IPALogger = IPA.Logging.Logger;

namespace BeatSaberMarkupLanguage
{
    // Internal as this is our instance of the logger.
    internal static class Logger
    {
        internal static IPALogger log { get; set; }
    }
}
