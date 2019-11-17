using System;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButton
    {
        [UIComponent("button")]
        internal Button button;

        //If you're wondering why I need another set of properties it's because attributes aren't inherited
        [UIValue("text")]
        public string _Text => Text;
        [UIValue("hover-hint")]
        public string _HoverHint => HoverHint;
        [UIValue("start-interactable")]
        public bool _StartInteractable => StartInteractable;

        public virtual string Text { get; protected set; }
        public virtual string HoverHint { get; protected set; }
        protected virtual bool StartInteractable => true;
        public virtual Action OnClick { get; protected set; }
        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }
        [UIAction("button-click")]
        public void _OnClick()
        {
            OnClick?.Invoke();
        }
        
        protected MenuButton() { }
        public MenuButton(string text, string hoverHint, Action onClick)
        {
            Text = text;
            HoverHint = hoverHint ?? string.Empty;
            OnClick = onClick;
        }

        public MenuButton(string text, Action onClick)
        : this(text, string.Empty, onClick)
        { }
    }
}
