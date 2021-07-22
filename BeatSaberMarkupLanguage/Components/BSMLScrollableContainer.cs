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
            get => Utilities.GetPrivateProperty<ScrollView, Button>(this, "_pageUpButton");
            set => Utilities.SetPrivateProperty(this, "_pageUpButton", value);
        }
        public Button PageDownButton
        {
            get => Utilities.GetPrivateProperty<ScrollView, Button>(this, "_pageDownButton");
            set => Utilities.SetPrivateProperty(this, "_pageDownButton", value);
        }

        public RectTransform Viewport
        {
            get => Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport");
            set => Utilities.SetPrivateProperty(this, "_viewport", value);
        }

        public RectTransform ContentRect
        {
            get => Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_contentRectTransform");
            set => Utilities.SetPrivateProperty(this, "_contentRectTransform", value);
        }

        public VerticalScrollIndicator ScrollIndicator
        {
            get => Utilities.GetPrivateProperty<ScrollView, VerticalScrollIndicator>(this, "_verticalScrollIndicator");
            set => Utilities.SetPrivateProperty(this, "_verticalScrollIndicator", value);
        }

        private bool alignBottom = false;
        public bool AlignBottom
        {
            get => alignBottom;
            set
            {
                alignBottom = value;
                ScrollTo(Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos"), true);
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
        private float scrollPageHeight => Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport").rect.height;

        private void UpdateViewportMask()
        {
            var img = Viewport.GetComponent<Image>();
            if (img != null)
                img.enabled = MaskOverflow;
        }

        public new void Awake()
        {
            Utilities.SetPrivateProperty(this, "_buttonBinder", new ButtonBinder());
            
            RefreshContent();
            RefreshButtons();
            runScrollAnim = false;
        }

        public void RefreshBindings()
        {
            ButtonBinder _buttonBinder = Utilities.GetPrivateProperty<ScrollView, ButtonBinder>(this, "_buttonBinder");
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
            SetContentSize(Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_contentRectTransform").rect.height);
            contentHeight = Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_contentRectTransform").rect.height;
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
            RectTransform _contentRectTransform = Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_contentRectTransform");
            if (contentHeight != _contentRectTransform.rect.height && _contentRectTransform.rect.height > 0f)
                ContentSizeUpdated();

            if (runScrollAnim)
            {
                float _destinationPos = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos");
                float num = Mathf.Lerp(_contentRectTransform.anchoredPosition.y, _destinationPos, Time.deltaTime * Utilities.GetPrivateProperty<ScrollView, float>(this, "_smooth"));
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
                PageUpButton.interactable = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") > 0f;
            if (PageDownButton != null)
                PageDownButton.interactable = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") < contentHeight - (Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport")?.rect.height ?? 0);
        }

        public new void ComputeScrollFocusPosY()
        {
            ItemForFocussedScrolling[] componentsInChildren = GetComponentsInChildren<ItemForFocussedScrolling>(true);
            Utilities.SetPrivateProperty(this, "_scrollFocusPositions", (from item in componentsInChildren
                                      select ((Vector2)Utilities.InvokePrivateMethod(this, "WorldPositionToScrollViewPosition", new object[] {item.transform.position})).y into i
                                      orderby i
                                      select i).ToArray<float>());
        }

        public new void UpdateVerticalScrollIndicator(float posY)
        {
            if (Utilities.GetPrivateProperty<ScrollView, VerticalScrollIndicator>(this, "_verticalScrollIndicator") != null)
            {
                Utilities.GetPrivateProperty<ScrollView, VerticalScrollIndicator>(this, "_verticalScrollIndicator").progress = posY / (contentHeight - Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport").rect.height);
            }
        }

        public new void ScrollDown(bool animated)
        {
            float dstPosY = contentHeight - Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport").rect.height;
            ScrollTo(dstPosY, animated);
        }

        public new void ScrollToWorldPosition(Vector3 worldPosition, float pageRelativePosition, bool animated)
        {
            float num = ((Vector2)Utilities.InvokePrivateMethod(this, "WorldPositionToScrollViewPosition", new object[] {worldPosition})).y;
            num -= pageRelativePosition * scrollPageHeight;
            ScrollTo(num, animated);
        }

        public new void ScrollToWorldPositionIfOutsideArea(Vector3 worldPosition, float pageRelativePosition, float relativeBoundaryStart, float relativeBoundaryEnd, bool animated)
        {
            float num = ((Vector2)Utilities.InvokePrivateMethod(this, "WorldPositionToScrollViewPosition", new object[] {worldPosition})).y;
            float num2 = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") + relativeBoundaryStart * scrollPageHeight;
            float num3 = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") + relativeBoundaryEnd * scrollPageHeight;
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
                Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_contentRectTransform").anchoredPosition = new Vector2(0f, Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos"));
            }
            RefreshButtons();
            runScrollAnim = true;
        }

        public new void PageUpButtonPressed()
        {
            float num = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos");
            switch (_scrollType)
            {
                case ScrollType.PageSize:
                    num -= Utilities.GetPrivateProperty<ScrollView, float>(this, "_pageStepNormalizedSize") * scrollPageHeight;
                    break;
                case ScrollType.FixedCellSize: {
                    num -= _fixedCellSize * (float)(Mathf.RoundToInt(scrollPageHeight / _fixedCellSize) - 1);
                    num = (float)Mathf.FloorToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                }
                case ScrollType.FocusItems:
                    {
                        float threshold = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") + _scrollItemRelativeThresholdPosition * scrollPageHeight;
                        num = (from posy in Utilities.GetPrivateProperty<ScrollView, float[]>(this, "_scrollFocusPositions")
                               where posy < threshold
                               select posy).DefaultIfEmpty(Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos")).Max();
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
            float num = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos");
            switch (_scrollType)
            {
                case ScrollType.PageSize:
                    num += _pageStepNormalizedSize * scrollPageHeight;
                    break;
                case ScrollType.FixedCellSize:
                    num += _fixedCellSize * (float)(Mathf.RoundToInt(scrollPageHeight / _fixedCellSize) - 1);
                    num = (float)Mathf.CeilToInt(num / _fixedCellSize) * _fixedCellSize;
                    break;
                case ScrollType.FocusItems:
                    {
                        float threshold = Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") + (1f - _scrollItemRelativeThresholdPosition) * scrollPageHeight;
                        num = (from posy in Utilities.GetPrivateProperty<ScrollView, float[]>(this, "_scrollFocusPositions")
                               where posy > threshold
                               select posy).DefaultIfEmpty(Utilities.GetPrivateProperty<ScrollView, float>(this, "_destinationPos") + scrollPageHeight).Min();
                        num -= (1f - _pageStepNormalizedSize) * scrollPageHeight;
                        break;
                    }
            }
            ScrollTo(num, true);
            RefreshButtons();
            enabled = true;
        }

        public new void SetDestinationPosY(float value)
        {
            float maxPosition = contentHeight - Utilities.GetPrivateProperty<ScrollView, RectTransform>(this, "_viewport").rect.height;
            if (maxPosition < 0 && !AlignBottom) maxPosition = 0f;
            Utilities.SetPrivateProperty(this, "_destinationPos", Mathf.Min(maxPosition, Mathf.Max(0f, value)));
        }
    }
}
