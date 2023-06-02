using System.Linq;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    // at this point this is a pseudo-reimplementation
    public class BSMLScrollableContainer : ScrollView
    {
        private bool alignBottom = false;
        private bool maskOverflow = true;
        private float contentHeight;
        private bool runScrollAnim = false;

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

        public bool AlignBottom
        {
            get => alignBottom;
            set
            {
                alignBottom = value;
                ScrollTo(_destinationPos, true);
            }
        }

        public bool ScrollToBottomOnUpdate { get; set; } = false;

        public bool MaskOverflow
        {
            get => maskOverflow;
            set
            {
                maskOverflow = value;
                UpdateViewportMask();
            }
        }

        private float ScrollPageHeight => _viewport.rect.height;

        public void RefreshBindings()
        {
            if (_buttonBinder == null)
            {
                return;
            }

            _buttonBinder.ClearBindings();

            if (PageUpButton != null)
            {
                _buttonBinder.AddBinding(PageUpButton, PageUpButtonPressed);
            }

            if (PageDownButton != null)
            {
                _buttonBinder.AddBinding(PageDownButton, PageDownButtonPressed);
            }
        }

        public void RefreshContent()
        {
            SetContentSize(_contentRectTransform.rect.height);
            contentHeight = _contentRectTransform.rect.height;
            RefreshBindings();
            ComputeScrollFocusPosY();
        }

        public void ContentSizeUpdated()
        {
            RefreshContent();
            RefreshButtons();
            ScrollTo(ScrollToBottomOnUpdate ? float.MaxValue : 0f, false);
        }

        public new void RefreshButtons()
        {
            if (PageUpButton != null)
            {
                PageUpButton.interactable = _destinationPos > 0f;
            }

            if (PageDownButton != null)
            {
                PageDownButton.interactable = _destinationPos < contentHeight - (_viewport != null ? _viewport.rect.height : 0);
            }
        }

        public void ComputeScrollFocusPosY()
        {
            ItemForFocussedScrolling[] componentsInChildren = GetComponentsInChildren<ItemForFocussedScrolling>(true);
            _scrollFocusPositions = (from item in componentsInChildren
                                     select WorldPositionToScrollViewPosition(item.transform.position).y into i
                                     orderby i
                                     select i).ToArray();
        }

        public new void UpdateVerticalScrollIndicator(float posY)
        {
            if (_verticalScrollIndicator != null)
            {
                _verticalScrollIndicator.progress = posY / (contentHeight - _viewport.rect.height);
            }
        }

        public void ScrollDown(bool animated)
        {
            float dstPosY = contentHeight - _viewport.rect.height;
            ScrollTo(dstPosY, animated);
        }

        public new void ScrollToWorldPosition(Vector3 worldPosition, float pageRelativePosition, bool animated)
        {
            float num = WorldPositionToScrollViewPosition(worldPosition).y;
            num -= pageRelativePosition * ScrollPageHeight;
            ScrollTo(num, animated);
        }

        public new void ScrollToWorldPositionIfOutsideArea(Vector3 worldPosition, float pageRelativePosition, float relativeBoundaryStart, float relativeBoundaryEnd, bool animated)
        {
            float num = WorldPositionToScrollViewPosition(worldPosition).y;
            float num2 = _destinationPos + (relativeBoundaryStart * ScrollPageHeight);
            float num3 = _destinationPos + (relativeBoundaryEnd * ScrollPageHeight);
            if (num > num2 && num < num3)
            {
                return;
            }

            num -= pageRelativePosition * ScrollPageHeight;
            ScrollTo(num, animated);
        }

        public new void ScrollTo(float dstPosY, bool animated)
        {
            SetDestinationPosY(dstPosY);
            if (!animated)
            {
                _contentRectTransform.anchoredPosition = new Vector2(0f, _destinationPos);
            }

            RefreshButtons();
            runScrollAnim = true;
        }

        public new void PageUpButtonPressed()
        {
            float num = _destinationPos;
            switch (_scrollType)
            {
                case ScrollType.PageSize:
                    num -= _pageStepNormalizedSize * ScrollPageHeight;
                    break;
                case ScrollType.FixedCellSize:
                    num -= _fixedCellSize * (Mathf.RoundToInt(ScrollPageHeight / _fixedCellSize) - 1);
                    num = Mathf.FloorToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                case ScrollType.FocusItems:
                    {
                        float threshold = _destinationPos + (_scrollItemRelativeThresholdPosition * ScrollPageHeight);
                        num = (from posy in _scrollFocusPositions
                               where posy < threshold
                               select posy).DefaultIfEmpty(_destinationPos).Max();
                        num -= _pageStepNormalizedSize * ScrollPageHeight;
                        break;
                    }
            }

            ScrollTo(num, true);
            RefreshButtons();
            enabled = true;
        }

        public new void PageDownButtonPressed()
        {
            float num = _destinationPos;
            switch (_scrollType)
            {
                case ScrollType.PageSize:
                    num += _pageStepNormalizedSize * ScrollPageHeight;
                    break;
                case ScrollType.FixedCellSize:
                    num += _fixedCellSize * (Mathf.RoundToInt(ScrollPageHeight / _fixedCellSize) - 1);
                    num = Mathf.CeilToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                case ScrollType.FocusItems:
                    {
                        float threshold = _destinationPos + ((1f - _scrollItemRelativeThresholdPosition) * ScrollPageHeight);
                        num = (from posy in _scrollFocusPositions
                               where posy > threshold
                               select posy).DefaultIfEmpty(_destinationPos + ScrollPageHeight).Min();
                        num -= (1f - _pageStepNormalizedSize) * ScrollPageHeight;
                        break;
                    }
            }

            ScrollTo(num, true);
            RefreshButtons();
            enabled = true;
        }

        public void SetDestinationPosY(float value)
        {
            float maxPosition = contentHeight - _viewport.rect.height;

            if (maxPosition < 0 && !AlignBottom)
            {
                maxPosition = 0f;
            }

            _destinationPos = Mathf.Min(maxPosition, Mathf.Max(0f, value));
        }

        private new void Awake()
        {
            _buttonBinder = new ButtonBinder();

            RefreshContent();
            RefreshButtons();
            runScrollAnim = false;
        }

        private new void Update()
        {
            if (contentHeight != _contentRectTransform.rect.height && _contentRectTransform.rect.height > 0f)
            {
                ContentSizeUpdated();
            }

            if (runScrollAnim)
            {
                float num = Mathf.Lerp(_contentRectTransform.anchoredPosition.y, _destinationPos, Time.deltaTime * _smooth);
                if (Mathf.Abs(num - _destinationPos) < 0.01f)
                {
                    num = _destinationPos;
                    runScrollAnim = false;
                }

                _contentRectTransform.anchoredPosition = new Vector2(0f, num);
                UpdateVerticalScrollIndicator(_contentRectTransform.anchoredPosition.y);
            }
        }

        private void UpdateViewportMask()
        {
            if (Viewport.TryGetComponent(out Image img))
            {
                img.enabled = MaskOverflow;
            }
        }
    }
}
