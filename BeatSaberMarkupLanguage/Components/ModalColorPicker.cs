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
        [SerializeField]
        private ModalView modalView;

        [SerializeField]
        private RGBPanelController rgbPanel;

        [SerializeField]
        private HSVPanelController hsvPanel;

        [SerializeField]
        private Image colorImage;

        private Color currentColor;

        public event Action<Color> DoneEvent;

        public event Action CancelEvent;

        public BSMLValue AssociatedValue { get; set; }

        public BSMLAction OnCancel { get; set; }

        public BSMLAction OnDone { get; set; }

        public BSMLAction OnChange { get; set; }

        public ModalView ModalView
        {
            get => modalView;
            set => modalView = value;
        }

        public RGBPanelController RgbPanel
        {
            get => rgbPanel;
            set => rgbPanel = value;
        }

        public HSVPanelController HsvPanel
        {
            get => hsvPanel;
            set => hsvPanel = value;
        }

        public Image ColorImage
        {
            get => colorImage;
            set => colorImage = value;
        }

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
            OnCancel?.Invoke();
            CancelEvent?.Invoke();
            modalView.Hide(true);
        }

        [UIAction("done")]
        public void DonePressed()
        {
            AssociatedValue?.SetValue(CurrentColor);
            OnDone?.Invoke(CurrentColor);
            DoneEvent?.Invoke(CurrentColor);
            modalView.Hide(true);
        }

        public void OnValueChanged(Color color, ColorChangeUIEventType type)
        {
            OnChange?.Invoke(color);
            CurrentColor = color;
        }

        protected void OnEnable()
        {
            if (AssociatedValue != null)
            {
                CurrentColor = (Color)AssociatedValue.GetValue();
            }
        }
    }
}
