using HMUI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSliderSetting : GenericInteractableSetting
    {
        private RangeValuesTextSlider _slider;
        public RangeValuesTextSlider slider
        {
            get
            {
                return _slider;
            }
            set
            {
                if (dragHelper != null)
                {
                    dragHelper.DragReleased -= OnDragReleased;
                    dragHelper.DragStarted -= OnDragStarted;
                    Destroy(dragHelper);
                }
                if(value != null)
                {
                    dragHelper = value.gameObject.AddComponent<DragHelper>();
                    dragHelper.DragReleased += OnDragReleased;
                    dragHelper.DragStarted += OnDragStarted;
                }
                _slider = value;
            }
        }

        public event EventHandler<PointerEventData> DragStarted;
        public event EventHandler<PointerEventData> DragReleased;

        protected TextMeshProUGUI text;
        protected DragHelper dragHelper;
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
        private void OnDragStarted(object s, PointerEventData e) => DragStarted?.Invoke(this, e);
        private void OnDragReleased(object s, PointerEventData e) => DragReleased?.Invoke(this, e);
    }
}
