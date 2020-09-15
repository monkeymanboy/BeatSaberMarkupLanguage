using HMUI;
using System;
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
        protected virtual void OnDragStarted(object s, PointerEventData e)
        {
            IsDragging = true;
            DragStarted?.Invoke(this, e);
        }
        protected virtual void OnDragReleased(object s, PointerEventData e)
        {
            IsDragging = false;
            DragReleased?.Invoke(this, e);
        }
    }
}
