using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButton : INotifyPropertyChanged
    {
        public virtual Action OnClick { get; protected set; }

        private string text;

        [UIValue("text")]
        public virtual string Text
        {
            get => text;
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }

        private string hoverHint;

        [UIValue("hover-hint")]
        public virtual string HoverHint
        {
            get => hoverHint;
            set
            {
                hoverHint = value;
                NotifyPropertyChanged();
            }
        }

        private bool interactable;

        [UIValue("interactable")]
        public virtual bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("button-click")]
        public void ButtonClicked()
        {
            OnClick?.Invoke();
        }

        protected MenuButton()
        {
        }

        public MenuButton(string text, string hoverHint, Action onClick, bool interactable = true)
        {
            Text = text;
            HoverHint = hoverHint ?? string.Empty;
            OnClick = onClick;
            Interactable = interactable;
        }

        public MenuButton(string text, Action onClick)
            : this(text, string.Empty, onClick)
        {
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
                Logger.Log?.Error($"Error Invoking PropertyChanged: {ex.Message}");
                Logger.Log?.Error(ex);
            }
        }
    }
}
