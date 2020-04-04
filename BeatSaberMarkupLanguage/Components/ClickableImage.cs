using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ClickableImage : Image, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
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
