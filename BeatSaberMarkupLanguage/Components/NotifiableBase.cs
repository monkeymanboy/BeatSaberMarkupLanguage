using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                Logger.Log?.Error($"Error Invoking PropertyChanged: {ex.Message}");
                Logger.Log?.Error(ex);
            }
        }
    }
}
