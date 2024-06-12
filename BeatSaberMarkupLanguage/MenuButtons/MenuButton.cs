using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButton : INotifyPropertyChanged
    {
        private string _text;
        private string _hoverHint;
        private bool _interactable;

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

        public virtual Action OnClick { get; protected set; }

        [UIValue("text")]
        public virtual string Text
        {
            get => _text;
            set
            {
                _text = value;
                StrippedText = Utilities.StripHtmlTags(_text);
                NotifyPropertyChanged();
            }
        }

        [UIValue("hover-hint")]
        public virtual string HoverHint
        {
            get => _hoverHint;
            set
            {
                _hoverHint = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("interactable")]
        public virtual bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
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
