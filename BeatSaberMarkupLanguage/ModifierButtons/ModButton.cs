/*
using System;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Notify;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.ModifierButtons
{
    public class ModButton : INotifiableHost
    {
        protected internal bool isActive = false;

        public virtual Action OnClick { get; protected set; }
        private string _text;
        [UIValue("text")]
        public virtual string Text {
            get => _text;
            set
            {
                _text = value;
                NotifyPropertyChanged();
            }
        }
        private string _hoverHint;
        [UIValue("hover-hint")]
        public virtual string HoverHint {
            get => _hoverHint;
            set
            {
                _hoverHint = value;
                NotifyPropertyChanged();
            }
        }
        private bool _interactable;
        [UIValue("interactable")]
        public virtual bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                NotifyPropertyChanged();
            }
        }
        [UIAction("button-click")]
        public void ButtonClicked()
        {
            OnClick?.Invoke();
        }
        
        protected ModButton() { }
        public ModButton(string text, string hoverHint, Action onClick, bool interactable = true)
        {
            Text = text;
            HoverHint = hoverHint ?? string.Empty;
            OnClick = onClick;
            Interactable = interactable;
        }

        public ModButton(string text, Action onClick)
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
*/