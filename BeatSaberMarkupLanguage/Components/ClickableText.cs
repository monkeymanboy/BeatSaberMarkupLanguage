using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components
{
    // Yoinked from CustomUI, it do be like that sometimes
    public class ClickableText : TextMeshProUGUI, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
    {
        public Color highlightColor = new Color(0.60f, 0.80f, 1);
        public Color defaultColor = Color.white;
        public Action<PointerEventData> OnClickEvent, PointerEnterEvent, PointerExitEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            color = defaultColor;
            OnClickEvent?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            color = highlightColor;
            PointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            color = defaultColor;
            PointerExitEvent?.Invoke(eventData);
        }
    }
}
