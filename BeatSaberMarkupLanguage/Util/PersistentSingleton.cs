namespace BeatSaberMarkupLanguage.Util
{
    // Created this for compatibility's sake after PersistentSingleton was removed from the game
    // but the idea is to eventually get rid of this and do everything through Zenject. This class
    // is much simpler and does not inherit from MonoBehaviour since we don't need it.
    public class PersistentSingleton<T>
        where T : class, new()
    {
        public static T instance { get; } = new();
    }
}
