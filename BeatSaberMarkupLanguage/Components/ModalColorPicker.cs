using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;

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

        private Color _currentColor;
        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                if(rgbPanel != null)
                    rgbPanel.color = _currentColor;
                if(hsvPanel != null && hsvPanel.color != _currentColor) //If you're wondering why we check this for hsv it's so that if color is one where changing hue has no effect it won't lock up the hue slider
                    hsvPanel.color = _currentColor;
                if(colorImage != null)
                    colorImage.color = _currentColor;
            }
        }

        void OnEnable()
        {
            if (associatedValue != null)
                CurrentColor = (Color)associatedValue.GetValue();
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
    }
}
