using HMUI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        public RangeValuesTextSlider slider;
        public event EventHandler<PointerEventData> DragStarted;
        public event EventHandler<PointerEventData> DragReleased;
        public bool IsDragging { get; protected set; }

        protected TextMeshProUGUI text;
        private DragHelper _dragHelper;
        public DragHelper dragHelper
        {
            get => _dragHelper;
            set
            {
                if(_dragHelper != null && _dragHelper != value)
                {
                    _dragHelper.DragStarted -= OnDragStarted;
                    _dragHelper.DragReleased -= OnDragReleased;
                }
                if(value != null)
                {
                    value.DragStarted += OnDragStarted;
                    value.DragReleased += OnDragReleased;
                }
                _dragHelper = value;
            }
        }
        public bool updateDuringDrag = true;
        public override bool interactable 
        { 
            get => slider?.interactable ?? false;
            set
            {
                if(slider != null)
                {
                    slider.interactable = value;
                }
            }
        }

        protected abstract IEnumerator SetInitialText();

        protected abstract void RaiseValueChanged(bool emitEvent);

        protected virtual void OnEnable()
        {
            StartCoroutine(SetInitialText());
        }

        protected virtual void OnChange(TextSlider _, float val)
        {
            // Check IsDragging for safety in case something goes wrong with DragHelper?
            bool emitEvent = !IsDragging || updateDuringDrag;
            RaiseValueChanged(emitEvent);
        }

        protected virtual void OnDragStarted(object s, PointerEventData e)
        {
            IsDragging = true;
            DragStarted?.Invoke(this, e);
        }
        protected virtual void OnDragReleased(object s, PointerEventData e)
        {
            IsDragging = false;
            DragReleased?.Invoke(this, e);
            // If updateDuringDrag is true, no need to raise the event again.
            RaiseValueChanged(!updateDuringDrag);
        }

    }
}
