using System;

namespace BeatSaberMarkupLanguage.Util
{
    /// <summary>
    /// Allows access to a singleton managed by Zenject.
    /// </summary>
    /// <remarks>Avoid using this class directly. Instead, access <c>Instance</c> on the derived class directly.</remarks>
    /// <typeparam name="T">The type that can be resolved by Zenject.</typeparam>
    public abstract class ZenjectSingleton<T>
        where T : class
    {
        internal ZenjectSingleton()
        {
        }

        /// <summary>
        /// Gets the current instance of this class registered with Zenject.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this property is accessed when the main menu hasn't yet initialized.</exception>
        /// <remarks>If you are using Zenject, consider injecting the instance of this class directly rather than using this property.</remarks>
        public static T Instance => BeatSaberUI.DiContainer != null && !BeatSaberUI.DiContainer.IsInstalling ? BeatSaberUI.DiContainer.Resolve<T>() : throw new InvalidOperationException($"Tried getting {typeof(T).Name} too early!");
    }
}
