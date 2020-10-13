using System;

namespace BeatSaberMarkupLanguage.Notify
{
    [Obsolete("Use System.ComponentModel.INotifyPropertyChanged.")]
    public interface INotifiableHost : System.ComponentModel.INotifyPropertyChanged
    {

    }

    // public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    [Obsolete("Use System.ComponentModel.PropertyChangedEventArgs.")]
    public class PropertyChangedEventArgs : System.ComponentModel.PropertyChangedEventArgs
    {
        public PropertyChangedEventArgs(string propertyName)
            : base(propertyName) { }
    }
}
