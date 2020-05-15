using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Notify
{
    public interface INotifiableHost
    {
        event PropertyChangedEventHandler PropertyChanged;
    }

    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public PropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
