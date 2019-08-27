using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components
{
    //Yoinked from customui, it do be like that sometimes
    public class ClickableText : TextMeshProUGUI, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
    {
        public Color highlightColor = Color.cyan;
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
