using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeatSaberMarkupLanguage.Util
{
    [Obsolete("Avoid using singletons.")]
    public class NotifiableSingleton<T> : PersistentSingleton<T>, INotifyPropertyChanged
        where T : class, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged for property '{propertyName}' on {GetType().FullName}\n{ex}");
            }
        }
    }
}
