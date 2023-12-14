namespace BeatSaberMarkupLanguage.Util
{
    internal static class ObjectExtensions
    {
        internal static string GetTypeFullNameSafe(this object obj) => obj is not null ? obj.GetType().FullName : "<null>";
    }
}
