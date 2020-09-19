using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DragHelper : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public event EventHandler<PointerEventData> DragStarted;
        public event EventHandler<PointerEventData> DragReleased;
        private DragStartedEvent _dragStartedEvent = new DragStartedEvent();
        private DragReleasedEvent _dragReleasedEvent = new DragReleasedEvent();

        public DragStartedEvent onDragStarted
        {
            get { return _dragStartedEvent; }
            set { _dragStartedEvent = value; }
        }
        public DragReleasedEvent onDragReleased
        {
            get { return _dragReleasedEvent; }
            set { _dragReleasedEvent = value; }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            DragStarted.RaiseEventSafe(this, eventData, nameof(DragStarted));
            onDragStarted?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragReleased.RaiseEventSafe(this, eventData, nameof(DragReleased));
            onDragReleased?.Invoke();
        }

        public class DragStartedEvent : UnityEvent
        {

        }
        public class DragReleasedEvent : UnityEvent
        {

        }
    }
}
