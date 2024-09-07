using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButton : INotifyPropertyChanged
    {
        private string text;
        private string hoverHint;
        private bool interactable;

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

        protected MenuButton()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action OnClick;

        [UIValue("text")]
        public virtual string Text
        {
            get => text;
            set
            {
                text = value;
                StrippedText = Utilities.StripHtmlTags(text);
                NotifyPropertyChanged();
            }
        }

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

        internal string StrippedText { get; private set; }

        [UIAction("button-click")]
        public void ButtonClicked()
        {
            OnClick?.Invoke();
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged for property '{propertyName}' on {nameof(MenuButtons)}\n{ex}");
            }
        }
    }
}
