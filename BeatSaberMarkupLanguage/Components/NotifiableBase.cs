using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Util;

namespace BeatSaberMarkupLanguage.Components
{
    /// <summary>
    /// Like <see cref="NotifiableSingleton{T}"/>, but without the persistent singleton.
    /// </summary>
    public class NotifiableBase : INotifyPropertyChanged
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
