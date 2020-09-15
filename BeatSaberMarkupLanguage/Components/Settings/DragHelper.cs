using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DragHelper : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public event EventHandler<PointerEventData> DragStarted;
        public event EventHandler<PointerEventData> DragReleased;

        public void OnBeginDrag(PointerEventData eventData) => DragStarted?.Invoke(this, eventData);

        public void OnEndDrag(PointerEventData eventData) => DragReleased?.Invoke(this, eventData);
    }
}
