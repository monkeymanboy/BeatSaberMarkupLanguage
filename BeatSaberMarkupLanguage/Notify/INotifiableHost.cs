using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Notify
{
    public interface INotifiableHost
    {
        event EventHandler<PropertyChangedEventArgs> PropertyChanged;
    }

    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public PropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
