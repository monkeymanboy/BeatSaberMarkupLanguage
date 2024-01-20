using System;
using System.Threading.Tasks;
using Zenject;

namespace BeatSaberMarkupLanguage.Util
{
    internal class MenuInitAwaiter : IInitializable, IDisposable
    {
        private static TaskCompletionSource<VoidResult> _taskCompletionSource = new();

        public void Initialize()
        {
            _taskCompletionSource.SetResult(default);
        }

        public void Dispose()
        {
            _taskCompletionSource = new();
        }

        internal static Task WaitForMainMenuAsync() => _taskCompletionSource.Task;
    }
}
