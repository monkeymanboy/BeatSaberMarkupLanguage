using System;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace BeatSaberMarkupLanguage.Util
{
    /// <summary>
    /// Contains utilities to wait for the main menu to initialize.
    /// </summary>
    /// <remarks>If you are using Zenject, you should not need to use this class. Instead, create a class that implements <see cref="IInitializable"/> and bind it to the main menu.</remarks>
    public class MainMenuAwaiter : IInitializable, IDisposable
    {
        private static TaskCompletionSource<VoidResult> taskCompletionSource = new();

        /// <summary>
        /// Occurs when the main menu is initializing.
        /// </summary>
        public static event Action MainMenuInitializing;

        /// <summary>
        /// Waits for the main menu to initialize asynchronously.
        /// </summary>
        /// <returns>A task that completes when the main menu is initializing.</returns>
        public static Task WaitForMainMenuAsync() => taskCompletionSource.Task;

        /// <inheritdoc />
        public void Initialize()
        {
            taskCompletionSource.SetResult(default);

            if (MainMenuInitializing != null)
            {
                foreach (Action action in MainMenuInitializing.GetInvocationList().Cast<Action>())
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error($"An exception occurred while invoking {nameof(MainMenuInitializing)}\n{ex}");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            taskCompletionSource = new();
        }
    }
}
