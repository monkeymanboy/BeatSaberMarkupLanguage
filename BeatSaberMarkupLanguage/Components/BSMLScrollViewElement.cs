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

        private bool maskOverflow = true;
        public bool MaskOverflow 
        {
            get => maskOverflow;
            set 
            {
                maskOverflow = value;
                UpdateViewportMask();
            } 
        }

        private void UpdateViewportMask()
        {
            var img = Viewport.GetComponent<Image>();
            if (img != null)
                img.enabled = MaskOverflow;
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
                PageDownButton.interactable = _dstPosY < _contentHeight - (_viewport?.rect.height ?? 0);
        }
        public override void ScrollDown(bool animated)
        {
            float dstPosY = this._contentHeight - this._viewport.rect.height;
            this.ScrollAt(dstPosY, animated);
        }

        public override void ScrollToWorldPosition(Vector3 worldPosition, float pageRelativePosition, bool animated)
        {
            float num = this.WorldPositionToScrollViewPosition(worldPosition).y;
            num -= pageRelativePosition * this._scrollPageHeight;
            this.ScrollAt(num, animated);
        }

        public override void ScrollToWorldPositionIfOutsideArea(Vector3 worldPosition, float pageRelativePosition, float relativeBoundaryStart, float relativeBoundaryEnd, bool animated)
        {
            float num = this.WorldPositionToScrollViewPosition(worldPosition).y;
            float num2 = this._dstPosY + relativeBoundaryStart * this._scrollPageHeight;
            float num3 = this._dstPosY + relativeBoundaryEnd * this._scrollPageHeight;
            if (num > num2 && num < num3)
            {
                return;
            }
            num -= pageRelativePosition * this._scrollPageHeight;
            this.ScrollAt(num, animated);
        }

        public override void ScrollAt(float dstPosY, bool animated)
        {
            this.SetDstPosY(dstPosY);
            if (!animated)
            {
                this._contentRectTransform.anchoredPosition = new Vector2(0f, this._dstPosY);
            }
            this.RefreshButtonsInteractibility();
            base.enabled = true;
        }

        public override void PageUpButtonPressed()
        {
            float threshold = this._dstPosY + this._scrollItemRelativeThresholdPosition * this._scrollPageHeight;
            float num = (from posy in this._scrollFocusPosYs
                         where posy < threshold
                         select posy).DefaultIfEmpty(this._dstPosY).Max();
            num -= this._pageStepRelativePosition * this._scrollPageHeight;
            this.SetDstPosY(num);
            this.RefreshButtonsInteractibility();
            base.enabled = true;
        }

        public override void PageDownButtonPressed()
        {
            float threshold = this._dstPosY + (1f - this._scrollItemRelativeThresholdPosition) * this._scrollPageHeight;
            float num = (from posy in this._scrollFocusPosYs
                         where posy > threshold
                         select posy).DefaultIfEmpty(this._dstPosY + this._scrollPageHeight).Min();
            num -= (1f - this._pageStepRelativePosition) * this._scrollPageHeight;
            this.SetDstPosY(num);
            this.RefreshButtonsInteractibility();
            base.enabled = true;
        }
    }
}
