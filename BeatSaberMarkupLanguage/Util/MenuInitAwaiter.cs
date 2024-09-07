using System;
using System.Threading.Tasks;
using Zenject;

namespace BeatSaberMarkupLanguage.Util
{
    internal class MenuInitAwaiter : IInitializable, IDisposable
    {
        private static TaskCompletionSource<VoidResult> taskCompletionSource = new();

        public void Initialize()
        {
            taskCompletionSource.SetResult(default);
        }

        public void Dispose()
        {
            taskCompletionSource = new();
        }

        internal static Task WaitForMainMenuAsync() => taskCompletionSource.Task;
    }
}
