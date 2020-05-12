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
    // at this point this is a pseudo-reimplementation
    public class BSMLScrollableContainer : ScrollView
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

        private bool alignBottom = false;
        public bool AlignBottom
        {
            get => alignBottom;
            set
            {
                alignBottom = value;
                ScrollAt(_dstPosY, true);
            }
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

        public new void Awake()
        {
            _buttonBinder = new ButtonBinder();

            Setup();
            RefreshButtonsInteractibility();
            runScrollAnim = false;
        }

        public new void Setup() => RefreshContent();

        public void RefreshBindings()
        {
            _buttonBinder.ClearBindings();
            if (PageUpButton != null)
                _buttonBinder.AddBinding(PageUpButton, PageUpButtonPressed);
            if (PageDownButton != null)
                _buttonBinder.AddBinding(PageDownButton, PageDownButtonPressed);
        }

        public void RefreshContent()
        {
            _contentHeight = _contentRectTransform.rect.height;
            _scrollPageHeight = _viewport.rect.height;
            bool active = _contentHeight > _viewport.rect.height;
            PageUpButton?.gameObject?.SetActive(active);
            PageDownButton?.gameObject?.SetActive(active);
            RefreshBindings();
            if (_verticalScrollIndicator != null)
            {
                _verticalScrollIndicator.gameObject.SetActive(active);
                _verticalScrollIndicator.normalizedPageHeight = _viewport.rect.height / _contentHeight;
            }
            ComputeScrollFocusPosY();
        }

        public void ContentSizeUpdated()
        {
            RefreshContent();
            RefreshButtonsInteractibility();
            ScrollAt(0f, false);
        }

        private bool runScrollAnim = false;
        public new void Update()
        {
            if (_contentHeight != _contentRectTransform.rect.height && _contentRectTransform.rect.height > 0f)
                ContentSizeUpdated();

            if (runScrollAnim)
            {
                float num = Mathf.Lerp(this._contentRectTransform.anchoredPosition.y, this._dstPosY, Time.deltaTime * this._smooth);
                if (Mathf.Abs(num - this._dstPosY) < 0.01f)
                {
                    num = this._dstPosY;
                    runScrollAnim = false;
                }
                this._contentRectTransform.anchoredPosition = new Vector2(0f, num);
                this.UpdateVerticalScrollIndicator(this._contentRectTransform.anchoredPosition.y);
            }
        }

        public new void RefreshButtonsInteractibility()
        {
            if (PageUpButton != null)
                PageUpButton.interactable = _dstPosY > 0f;
            if (PageDownButton != null)
                PageDownButton.interactable = _dstPosY < _contentHeight - (_viewport?.rect.height ?? 0);
        }

        public new void ComputeScrollFocusPosY()
        {
            ItemForFocussedScrolling[] componentsInChildren = base.GetComponentsInChildren<ItemForFocussedScrolling>(true);
            this._scrollFocusPosYs = (from item in componentsInChildren
                                      select this.WorldPositionToScrollViewPosition(item.transform.position).y into i
                                      orderby i
                                      select i).ToArray<float>();
        }

        public new void UpdateVerticalScrollIndicator(float posY)
        {
            if (this._verticalScrollIndicator != null)
            {
                this._verticalScrollIndicator.progress = posY / (this._contentHeight - this._viewport.rect.height);
            }
        }

        public new void ScrollDown(bool animated)
        {
            float dstPosY = this._contentHeight - this._viewport.rect.height;
            this.ScrollAt(dstPosY, animated);
        }

        public new void ScrollToWorldPosition(Vector3 worldPosition, float pageRelativePosition, bool animated)
        {
            float num = this.WorldPositionToScrollViewPosition(worldPosition).y;
            num -= pageRelativePosition * this._scrollPageHeight;
            this.ScrollAt(num, animated);
        }

        public new void ScrollToWorldPositionIfOutsideArea(Vector3 worldPosition, float pageRelativePosition, float relativeBoundaryStart, float relativeBoundaryEnd, bool animated)
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

        public new void ScrollAt(float dstPosY, bool animated)
        {
            this.SetDstPosY(dstPosY);
            if (!animated)
            {
                this._contentRectTransform.anchoredPosition = new Vector2(0f, this._dstPosY);
            }
            this.RefreshButtonsInteractibility();
            runScrollAnim = true;
        }

        public new void PageUpButtonPressed()
        {
            float threshold = this._dstPosY + this._scrollItemRelativeThresholdPosition * this._scrollPageHeight;
            float num = (from posy in this._scrollFocusPosYs
                         where posy < threshold
                         select posy).DefaultIfEmpty(this._dstPosY).Max();
            num -= this._pageStepRelativePosition * this._scrollPageHeight;
            this.SetDstPosY(num);
            this.RefreshButtonsInteractibility();
            runScrollAnim = true;
        }

        public new void PageDownButtonPressed()
        {
            float threshold = this._dstPosY + (1f - this._scrollItemRelativeThresholdPosition) * this._scrollPageHeight;
            float num = (from posy in this._scrollFocusPosYs
                         where posy > threshold
                         select posy).DefaultIfEmpty(this._dstPosY + this._scrollPageHeight).Min();
            num -= (1f - this._pageStepRelativePosition) * this._scrollPageHeight;
            this.SetDstPosY(num);
            this.RefreshButtonsInteractibility();
            runScrollAnim = true;
        }

        public new void SetDstPosY(float value)
        {
            float maxPosition = _contentHeight - _viewport.rect.height;
            if (maxPosition < 0 && !AlignBottom) maxPosition = 0f;
            this._dstPosY = Mathf.Min(maxPosition, Mathf.Max(0f, value));
        }
    }
}
