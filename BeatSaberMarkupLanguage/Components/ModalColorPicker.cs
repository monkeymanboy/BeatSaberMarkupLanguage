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
        public ModalView modalView;
        public RGBPanelController rgbPanel;
        public HSVPanelController hsvPanel;
        public Image colorImage;

        public BSMLValue associatedValue;
        public BSMLAction onCancel;
        public BSMLAction onDone;
        public BSMLAction onChange;

        public Action<Color> doneEvent;
        public Action cancelEvent;

        private Color currentColor;

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                if (rgbPanel != null)
                {
                    rgbPanel.color = currentColor;
                }

                // If you're wondering why we check this for HSV it's so that if color is one where changing hue has no effect it won't lock up the hue slider
                if (hsvPanel != null && hsvPanel.color != currentColor)
                {
                    hsvPanel.color = currentColor;
                }

                if (colorImage != null)
                {
                    colorImage.color = currentColor;
                }
            }
        }

        [UIAction("cancel")]
        public void CancelPressed()
        {
            onCancel?.Invoke();
            cancelEvent?.Invoke();
            modalView.Hide(true);
        }

        [UIAction("done")]
        public void DonePressed()
        {
            associatedValue?.SetValue(CurrentColor);
            onDone?.Invoke(CurrentColor);
            doneEvent?.Invoke(CurrentColor);
            modalView.Hide(true);
        }

        public void OnChange(Color color, ColorChangeUIEventType type)
        {
            onChange?.Invoke(color);
            CurrentColor = color;
        }

        private void OnEnable()
        {
            if (associatedValue != null)
            {
                CurrentColor = (Color)associatedValue.GetValue();
            }
        }
    }
}
