using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HMUI;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLViewController : ViewController, INotifyPropertyChanged
    {
        public Action<bool, bool, bool> didActivate;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string Content { get; }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                BSMLParser.instance.Parse(Content, gameObject, this);
            }

            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged for property '{propertyName}' on View Controller {name}\n{ex}");
            }
        }
    }
}
