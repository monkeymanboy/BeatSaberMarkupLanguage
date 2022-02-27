using HMUI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components
{
    public class ClickableImage : ImageView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private BasicUIAudioManager _basicUIAudioManager;
        
        private Color _highlightColor = new Color(0.60f, 0.80f, 1);
        public Color HighlightColor
        {
            get => _highlightColor;
            set
            {
                _highlightColor = value;
                UpdateHighlight();
            }
        }
        private Color _defaultColor = Color.white;
        public Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                UpdateHighlight();
            }
        }
        public Action<PointerEventData> OnClickEvent, PointerEnterEvent, PointerExitEvent;

        private bool _isHighlighted = false;
        private bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
                UpdateHighlight();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsHighlighted = false;
            OnClickEvent?.Invoke(eventData);
            if (_basicUIAudioManager == null)
            {
                _basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().First();
            }
            _basicUIAudioManager.HandleButtonClickEvent();
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
