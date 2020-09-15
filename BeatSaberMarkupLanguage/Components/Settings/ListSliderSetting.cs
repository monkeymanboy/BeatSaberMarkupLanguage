using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class ListSliderSetting : GenericSliderSetting
    {
        public List<object> values;
        public bool updateDuringDrag = true;

        public object Value
        {
            get => values[(int)Math.Round(slider.value)];
            set
            {
                slider.value = values.IndexOf(value) * 1f;
                text.text = TextForValue(Value);
            }
        }

        public override void Setup()
        {
            slider.minValue = 0;
            slider.maxValue = values.Count() - 1;
            text = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.numberOfSteps = values.Count;
            ReceiveValue();
            slider.valueDidChangeEvent += OnChange;
            if (isActiveAndEnabled)
                StartCoroutine(SetInitialText());
        }

        private void OnEnable()
        {
            StartCoroutine(SetInitialText());
        }

        // I don't really like this but for some reason I can't get the initial starting text any other quick way and this works perfectly fine
        private IEnumerator SetInitialText()
        {
            yield return new WaitForFixedUpdate();
            text.text = TextForValue(Value);
            yield return new WaitForSeconds(0.1f); // If the first one is too fast, don't yell at me pls
            text.text = TextForValue(Value);
        }

        private void RaiseValueChanged(bool emitEvent)
        {
            text.text = TextForValue(Value);
            if (emitEvent)
            {
                onChange?.Invoke(Value);
                if (updateOnChange)
                    ApplyValue();
            }
        }

        private void OnChange(TextSlider _, float val)
        {
            bool emitEvent = !IsDragging || updateDuringDrag;
            RaiseValueChanged(emitEvent);
        }

        protected override void OnDragReleased(object s, PointerEventData e)
        {
            base.OnDragReleased(s, e);
            // If updateDuringDrag is true, no need to raise the event again.
            RaiseValueChanged(!updateDuringDrag);
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
                Value = associatedValue.GetValue();
        }

        protected string TextForValue(object value)
        {
            return formatter == null ? value.ToString() : (formatter.Invoke(value) as string);
        }
    }
}