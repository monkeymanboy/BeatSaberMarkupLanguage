using System;
using HMUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components
{
    public class ClickableText : CurvedTextMeshPro, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<PointerEventData> OnClickEvent;
        public Action<PointerEventData> PointerEnterEvent;
        public Action<PointerEventData> PointerExitEvent;

        private Color highlightColor = new Color(0.60f, 0.80f, 1);
        private Color defaultColor = Color.white;
        private bool isHighlighted = false;

        public Color HighlightColor
        {
            get => highlightColor;
            set
            {
                highlightColor = value;
                UpdateHighlight();
            }
        }

        public Color DefaultColor
        {
            get => defaultColor;
            set
            {
                defaultColor = value;
                UpdateHighlight();
            }
        }

        private bool IsHighlighted
        {
            get => isHighlighted;
            set
            {
                isHighlighted = value;
                UpdateHighlight();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsHighlighted = false;
            OnClickEvent?.Invoke(eventData);
            BeatSaberUI.BasicUIAudioManager.HandleButtonClickEvent();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHighlighted = true;
            PointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHighlighted = false;
            PointerExitEvent?.Invoke(eventData);
        }

        private void UpdateHighlight()
        {
            color = IsHighlighted ? HighlightColor : DefaultColor;
        }
    }
}
