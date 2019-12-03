using System;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Notify;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButton : INotifiableHost
    {
        //If you're wondering why I need another set of properties it's because attributes aren't inherited
        [UIValue("text")]
        public string _Text => Text;
        [UIValue("hover-hint")]
        public string _HoverHint => HoverHint;
        [UIValue("interactable")]
        public bool _Interactable => Interactable;

        public virtual string Text { get; protected set; }
        public virtual string HoverHint { get; protected set; }
        public virtual Action OnClick { get; protected set; }
        private bool _interactable;
        public virtual bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                NotifyPropertyChanged("_Interactable");
            }
        }
        [UIAction("button-click")]
        public void _OnClick()
        {
            OnClick?.Invoke();
        }
        
        protected MenuButton() { }
        public MenuButton(string text, string hoverHint, Action onClick, bool interactable = true)
        {
            Text = text;
            HoverHint = hoverHint ?? string.Empty;
            OnClick = onClick;
            Interactable = interactable;
        }

        public MenuButton(string text, Action onClick)
        : this(text, string.Empty, onClick)
        { }

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
