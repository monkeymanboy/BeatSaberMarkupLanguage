using HMUI;
using System.Linq;
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
                ScrollTo(_destinationPos, true);
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

        private float contentHeight;
        private float scrollPageHeight => _viewport.rect.height;

        private void UpdateViewportMask()
        {
            var img = Viewport.GetComponent<Image>();
            if (img != null)
                img.enabled = MaskOverflow;
        }

        public new void Awake()
        {
            _buttonBinder = new ButtonBinder();

            RefreshContent();
            RefreshButtons();
            runScrollAnim = false;
        }

        public void RefreshBindings()
        {
            if (_buttonBinder == null)
            {
                return;
            }

            _buttonBinder.ClearBindings();
            if (PageUpButton != null)
                _buttonBinder.AddBinding(PageUpButton, PageUpButtonPressed);
            if (PageDownButton != null)
                _buttonBinder.AddBinding(PageDownButton, PageDownButtonPressed);
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
            ScrollTo(0f, false);
        }

        private bool runScrollAnim = false;
        public new void Update()
        {
            if (contentHeight != _contentRectTransform.rect.height && _contentRectTransform.rect.height > 0f)
                ContentSizeUpdated();

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

        public new void RefreshButtons()
        {
            if (PageUpButton != null)
                PageUpButton.interactable = _destinationPos > 0f;
            if (PageDownButton != null)
                PageDownButton.interactable = _destinationPos < contentHeight - (_viewport?.rect.height ?? 0);
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
            num -= pageRelativePosition * scrollPageHeight;
            ScrollTo(num, animated);
        }

        public new void ScrollToWorldPositionIfOutsideArea(Vector3 worldPosition, float pageRelativePosition, float relativeBoundaryStart, float relativeBoundaryEnd, bool animated)
        {
            float num = WorldPositionToScrollViewPosition(worldPosition).y;
            float num2 = _destinationPos + relativeBoundaryStart * scrollPageHeight;
            float num3 = _destinationPos + relativeBoundaryEnd * scrollPageHeight;
            if (num > num2 && num < num3)
            {
                return;
            }
            num -= pageRelativePosition * scrollPageHeight;
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
                    num -= _pageStepNormalizedSize * scrollPageHeight;
                    break;
                case ScrollType.FixedCellSize:
                    num -= _fixedCellSize * (Mathf.RoundToInt(scrollPageHeight / _fixedCellSize) - 1);
                    num = Mathf.FloorToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                case ScrollType.FocusItems:
                    {
                        float threshold = _destinationPos + _scrollItemRelativeThresholdPosition * scrollPageHeight;
                        num = (from posy in _scrollFocusPositions
                               where posy < threshold
                               select posy).DefaultIfEmpty(_destinationPos).Max();
                        num -= _pageStepNormalizedSize * scrollPageHeight;
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
                    num += _pageStepNormalizedSize * scrollPageHeight;
                    break;
                case ScrollType.FixedCellSize:
                    num += _fixedCellSize * (Mathf.RoundToInt(scrollPageHeight / _fixedCellSize) - 1);
                    num = Mathf.CeilToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                case ScrollType.FocusItems:
                    {
                        float threshold = _destinationPos + (1f - _scrollItemRelativeThresholdPosition) * scrollPageHeight;
                        num = (from posy in _scrollFocusPositions
                               where posy > threshold
                               select posy).DefaultIfEmpty(_destinationPos + scrollPageHeight).Min();
                        num -= (1f - _pageStepNormalizedSize) * scrollPageHeight;
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
            if (maxPosition < 0 && !AlignBottom) maxPosition = 0f;
            _destinationPos = Mathf.Min(maxPosition, Mathf.Max(0f, value));
        }
    }
}
