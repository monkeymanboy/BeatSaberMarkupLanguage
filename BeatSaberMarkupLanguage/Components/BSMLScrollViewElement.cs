using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLScrollViewElement : ScrollView
    {
        public Button PageUpButton
        {
            get => _pageUpButton;
            set => _pageUpButton = value;
        }
        public Button PageDownButton
        {
            get => _pageDownButton;
            set => _pageDownButton = value;
        }

        public RectTransform Viewport
        {
            get => _viewport;
            set => _viewport = value;
        }

        public RectTransform ContentRect
        {
            get => _contentRectTransform;
            set => _contentRectTransform = value;
        }

        public VerticalScrollIndicator ScrollIndicator
        {
            get => _verticalScrollIndicator;
            set => _verticalScrollIndicator = value;
        }

        public override void Awake()
        {
            _buttonBinder = new ButtonBinder();

            Setup();
            RefreshButtonsInteractibility();
            enabled = false;
        }

        public override void Setup()
        {
            _buttonBinder.ClearBindings();
            if (PageUpButton != null)
                _buttonBinder.AddBinding(PageUpButton, PageUpButtonPressed);
            if (PageDownButton != null)
                _buttonBinder.AddBinding(PageDownButton, PageDownButtonPressed);

            _contentHeight = _contentRectTransform.rect.height;
            _scrollPageHeight = _viewport.rect.height;
            bool active = _contentHeight > _viewport.rect.height;
            PageUpButton?.gameObject?.SetActive(active);
            PageDownButton?.gameObject?.SetActive(active);
            if (_verticalScrollIndicator != null)
            {
                _verticalScrollIndicator.gameObject.SetActive(active);
                _verticalScrollIndicator.normalizedPageHeight = _viewport.rect.height / _contentHeight;
            }
            ComputeScrollFocusPosY();
        }

        public override void RefreshButtonsInteractibility()
        {
            if (PageUpButton != null)
                PageUpButton.interactable = _dstPosY > 0f;
            if (PageDownButton != null)
                PageDownButton.interactable = _dstPosY < _contentHeight - _viewport.rect.height;
        }
    }
}
