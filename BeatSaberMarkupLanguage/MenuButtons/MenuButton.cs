using BeatSaberMarkupLanguage.Attributes;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public abstract class MenuButton
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
        
        public abstract string Text { get; }
        public abstract string HoverHint { get; }
        public virtual bool StartInteractable => true;
        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }
        [UIAction("button-click")]
        public void _OnClick()
        {
            OnClick();
        }
        public abstract void OnClick();
    }
}
