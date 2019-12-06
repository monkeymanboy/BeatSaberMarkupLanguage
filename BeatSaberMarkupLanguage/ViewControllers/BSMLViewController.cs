using BeatSaberMarkupLanguage.Notify;
using HMUI;
using System;
using System.Runtime.CompilerServices;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLViewController : ViewController, INotifiableHost
    {
        public abstract string Content { get; }

        public Action<bool, ActivationType> didActivate;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation)
                BSMLParser.instance.Parse(Content, gameObject, this);

            didActivate?.Invoke(firstActivation, type);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.log?.Error($"Error Invoking PropertyChanged: {ex.Message}");
                Logger.log?.Error(ex);
            }
        }
    }
}
