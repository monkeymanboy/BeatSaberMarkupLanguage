namespace BeatSaberMarkupLanguage.Util
{
    // Created this for compatibility's sake after PersistentSingleton was removed from the game
    // but the idea is to eventually get rid of this and do everything through Zenject. This class
    // is much simpler and does not inherit from MonoBehaviour since we don't need it.
    [System.Obsolete("Avoid using singletons.")]
    public class PersistentSingleton<T>
        where T : class, new()
    {
#pragma warning disable IDE1006, SA1300
        public static T instance { get; } = new();
#pragma warning restore IDE1006, SA1300
    }
}
