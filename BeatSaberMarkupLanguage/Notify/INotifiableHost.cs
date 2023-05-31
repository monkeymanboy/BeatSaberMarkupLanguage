using System;

namespace BeatSaberMarkupLanguage.Notify
{
    [Obsolete("Use System.ComponentModel.INotifyPropertyChanged.")]
    public interface INotifiableHost : System.ComponentModel.INotifyPropertyChanged
    {
    }

    [Obsolete("Use System.ComponentModel.PropertyChangedEventArgs.")]
    public class PropertyChangedEventArgs : System.ComponentModel.PropertyChangedEventArgs
    {
        public PropertyChangedEventArgs(string propertyName)
            : base(propertyName)
        {
        }
    }
}
