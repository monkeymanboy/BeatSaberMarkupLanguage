using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeatSaberMarkupLanguage.Util
{
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
                Logger.Log?.Error($"Error invoking PropertyChanged\n{ex}");
            }
        }
    }
}
