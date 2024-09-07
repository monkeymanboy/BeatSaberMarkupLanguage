using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ModalColorPicker : MonoBehaviour
    {
        public ModalView ModalView;
        public RGBPanelController RgbPanel;
        public HSVPanelController HsvPanel;
        public Image ColorImage;

        public BSMLValue AssociatedValue;
        public BSMLAction OnCancel;
        public BSMLAction OnDone;
        public BSMLAction OnChange;

        public Action<Color> DoneEvent;
        public Action CancelEvent;

        private Color currentColor;

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                if (RgbPanel != null)
                {
                    RgbPanel.color = currentColor;
                }

                // If you're wondering why we check this for HSV it's so that if color is one where changing hue has no effect it won't lock up the hue slider
                if (HsvPanel != null && HsvPanel.color != currentColor)
                {
                    HsvPanel.color = currentColor;
                }

                if (ColorImage != null)
                {
                    ColorImage.color = currentColor;
                }
            }
        }

        [UIAction("cancel")]
        public void CancelPressed()
        {
            OnCancel?.Invoke();
            CancelEvent?.Invoke();
            ModalView.Hide(true);
        }

        [UIAction("done")]
        public void DonePressed()
        {
            AssociatedValue?.SetValue(CurrentColor);
            OnDone?.Invoke(CurrentColor);
            DoneEvent?.Invoke(CurrentColor);
            ModalView.Hide(true);
        }

        public void OnValueChanged(Color color, ColorChangeUIEventType type)
        {
            OnChange?.Invoke(color);
            CurrentColor = color;
        }

        private void OnEnable()
        {
            if (AssociatedValue != null)
            {
                CurrentColor = (Color)AssociatedValue.GetValue();
            }
        }
    }
}
