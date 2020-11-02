using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BeatSaberMarkupLanguage.Components
{
    [RequireComponent(typeof(RectTransform), typeof(RectMask2D))]
    public class ScrollingText : MonoBehaviour
    {
        public RectTransform rectTransform { get; private set; }
        public TextMeshProUGUI textComponent { get; private set; }

        private string _cachedText = "";

        public ScrollMovementType movementType { get; set; } = ScrollMovementType.ByDuration;
        private ScrollAnimationType _animationType = ScrollAnimationType.Basic;
        public ScrollAnimationType animationType
        {
            get => _animationType;
            set
            {
                if (_animationType == value)
                    return;

                _animationType = value;

                if (_scrollAnimationCoroutine != null)
                {
                    StopCoroutine(_scrollAnimationCoroutine);
                    StartAnimation();
                }
            }
        }
        private float _textWidthRatioThreshold = 1.2f;
        /// <summary>
        /// The minimum ratio of text width to container width before scrolling occurs.
        /// Otherwise, if the text is wider than the container, the text will be scaled down to fit the container.
        /// </summary>
        public float textWidthRatioThreshold
        {
            get => _textWidthRatioThreshold;
            set
            {
                if (_textWidthRatioThreshold == value || value < 1f)
                    return;

                _textWidthRatioThreshold = value;
            }
        }
        private WaitForSeconds _pauseDuration = new WaitForSeconds(2f);
        /// <summary>
        /// The number of seconds to wait before the scrolling animation starts and ends.
        /// </summary>
        public float pauseDuration
        {
            set
            {
                if (value > 0f)
                    _pauseDuration = new WaitForSeconds(value);
            }
        }
        private float _scrollDuration = 2f;
        /// <summary>
        /// The number of seconds it takes to scroll the text.
        /// </summary>
        public float scrollDuration
        {
            get => _scrollDuration;
            set
            {
                if (value > 0f)
                    _scrollDuration = value;
            }
        }
        private float _scrollSpeed = 20f;
        /// <summary>
        /// The speed at which the text scrolls in units per second.
        /// </summary>
        public float scrollSpeed
        {
            get => _scrollSpeed;
            set
            {
                if (value > 0f)
                    _scrollSpeed = value;
            }
        }
        public bool alwaysScroll { get; set; } = false;

        private float _cachedTextWidth = 0f;
        private float TextWidth
        {
            get
            {
                if (_cachedText != textComponent.text)
                {
                    _cachedTextWidth = textComponent.GetPreferredValues().x;
                    _cachedText = textComponent.text;
                }

                return _cachedTextWidth;
            }
        }
        private float TextWidthRatio => rectTransform.rect.width != 0f ? TextWidth / rectTransform.rect.width : 0f;

        private IEnumerator _scrollAnimationCoroutine;

        private const float ScalingMinimumSizeRatio = 0.98f;
        private const float FadeDurationSeconds = 0.6f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            textComponent = BeatSaberUI.CreateText(rectTransform, "", Vector2.zero);
            var rt = textComponent.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector2.zero;

            // allow animation to restart when text has changed
            textComponent.RegisterDirtyLayoutCallback(OnTextComponentDirtyLayout);
        }

        private void OnEnable()
        {
            RecalculateElements(true);
        }

        private void OnDisable()
        {
            if (_scrollAnimationCoroutine != null)
            {
                StopCoroutine(_scrollAnimationCoroutine);
                _scrollAnimationCoroutine = null;
            }
        }

        private void OnTextComponentDirtyLayout()
        {
            if (textComponent.text != _cachedText)
                StartCoroutine(DelayedRecalculateElements());
        }

        private void OnRectTransformDimensionsChange()
        {
            RecalculateElements(true);
        }

        private void RecalculateElements(bool force = false)
        {
            // do not reset the animation if the text has not changed
            // _cachedText will be set when accessing TextWidth
            if (textComponent.text == _cachedText && !force)
                return;

            if (_scrollAnimationCoroutine != null)
            {
                StopCoroutine(_scrollAnimationCoroutine);
                _scrollAnimationCoroutine = null;
            }

            var rt = textComponent.rectTransform;
            rt.anchoredPosition = Vector2.zero;
            if (TextWidthRatio >= _textWidthRatioThreshold || alwaysScroll)
            {
                // resize the text component's RectTransform to take on the width of the text
                // otherwise, if the text is too long, it will disappear
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.sizeDelta = new Vector2(TextWidth, 0f);

                // position is set in the start of the animation
                StartAnimation();
            }
            else
            {
                // if we're changing the text here, this function will fire once again,
                // but that should be fine
                if (TextWidthRatio > ScalingMinimumSizeRatio)
                {
                    string scaleString = ((int)(100 * (ScalingMinimumSizeRatio + 0.001f) / TextWidthRatio)).InvariantToString("N0");
                    textComponent.text = $"<size={scaleString}%>" + textComponent.text + "</size>";
                }

                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
            }
        }

        private IEnumerator DelayedRecalculateElements(bool force = false)
        {
            yield return null;
            RecalculateElements(force);
        }

        private void StartAnimation()
        {
            if (animationType == ScrollAnimationType.FadeInOut)
                _scrollAnimationCoroutine = FadeScrollAnimationCoroutine();
            else if (animationType == ScrollAnimationType.ForwardAndReverse)
                _scrollAnimationCoroutine = ForwardReverseScrollAnimationCoroutine();
            else if (animationType == ScrollAnimationType.Continuous)
                _scrollAnimationCoroutine = ContinuousScrollAnimationCoroutine();
            else
                _scrollAnimationCoroutine = BasicScrollAnimationCoroutine();

            StartCoroutine(_scrollAnimationCoroutine);
        }

        #region Animation Coroutines
        private IEnumerator ScrollAnimationCoroutine(Vector2 startPos, Vector2 endPos)
        {
            RectTransform rt = textComponent.rectTransform;

            if (movementType == ScrollMovementType.ByDuration)
            {
                Vector2 nextPos = startPos;
                float seconds = 0f;

                if (startPos.x > endPos.x)
                {
                    // left to right
                    while (nextPos.x > endPos.x)
                    {
                        nextPos.x = Mathf.Lerp(startPos.x, endPos.x, seconds / scrollDuration);
                        rt.anchoredPosition = nextPos;
                        seconds += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    // right to left
                    while (nextPos.x < endPos.x)
                    {
                        nextPos.x = Mathf.Lerp(startPos.x, endPos.x, seconds / scrollDuration);
                        rt.anchoredPosition = nextPos;
                        seconds += Time.deltaTime;
                        yield return null;
                    }
                }
            }
            else if (movementType == ScrollMovementType.BySpeed)
            {
                Vector2 nextPos = startPos;
                if (startPos.x > endPos.x)
                {
                    // left to right
                    nextPos.x -= Time.deltaTime * scrollSpeed;
                    while (nextPos.x > endPos.x)
                    {
                        rt.anchoredPosition = nextPos;
                        yield return null;
                        nextPos.x -= Time.deltaTime * scrollSpeed;
                    }
                }
                else
                {
                    // right to left
                    nextPos.x += Time.deltaTime * scrollSpeed;
                    while (nextPos.x < endPos.x)
                    {
                        rt.anchoredPosition = nextPos;
                        yield return null;
                        nextPos.x += Time.deltaTime * scrollSpeed;
                    }
                }
            }
        }

        private IEnumerator BasicScrollAnimationCoroutine()
        {
            RectTransform rt = this.textComponent.rectTransform;
            float halfTextWidth = TextWidth / 2f;
            float halfWidth = rectTransform.rect.width / 2f;
            Vector2 startPos = new Vector2(halfTextWidth - halfWidth, 0f);
            Vector2 endPos = new Vector2(halfWidth - halfTextWidth, 0f);

            while (true)
            {
                rt.anchoredPosition = startPos;
                yield return _pauseDuration;

                IEnumerator anim = ScrollAnimationCoroutine(startPos, endPos);
                while (anim.MoveNext())
                    yield return anim.Current;

                rt.anchoredPosition = endPos;
                yield return _pauseDuration;
            }
        }

        private IEnumerator FadeScrollAnimationCoroutine()
        {
            RectTransform rt = this.textComponent.rectTransform;
            float halfTextWidth = TextWidth / 2f;
            float halfWidth = rectTransform.rect.width / 2f;
            Vector2 startPos = new Vector2(halfTextWidth - halfWidth, 0f);
            Vector2 endPos = new Vector2(halfWidth - halfTextWidth, 0f);

            while (true)
            {
                rt.anchoredPosition = startPos;

                // fade in
                float seconds = 0f;
                while (seconds < FadeDurationSeconds)
                {
                    yield return null;

                    Color currentColor = textComponent.color;
                    currentColor.a = Mathf.Lerp(0f, 1f, seconds / FadeDurationSeconds);
                    textComponent.color = currentColor;

                    seconds += Time.deltaTime;
                }
                yield return _pauseDuration;

                IEnumerator anim = ScrollAnimationCoroutine(startPos, endPos);
                while (anim.MoveNext())
                    yield return anim.Current;

                rt.anchoredPosition = endPos;
                yield return _pauseDuration;

                // fade out
                seconds = 0f;
                while (seconds < FadeDurationSeconds)
                {
                    Color currentColor = textComponent.color;
                    currentColor.a = Mathf.Lerp(1f, 0f, seconds / FadeDurationSeconds);
                    textComponent.color = currentColor;

                    seconds += Time.deltaTime;
                    yield return null;
                }
            }
        }

        private IEnumerator ForwardReverseScrollAnimationCoroutine()
        {
            RectTransform rt = this.textComponent.rectTransform;
            float halfTextWidth = TextWidth / 2f;
            float halfWidth = rectTransform.rect.width / 2f;
            Vector2 startPos = new Vector2(halfTextWidth - halfWidth, 0f);
            Vector2 endPos = new Vector2(halfWidth - halfTextWidth, 0f);

            while (true)
            {
                rt.anchoredPosition = startPos;
                yield return _pauseDuration;

                IEnumerator anim = ScrollAnimationCoroutine(startPos, endPos);
                while (anim.MoveNext())
                    yield return anim.Current;

                rt.anchoredPosition = endPos;
                yield return _pauseDuration;

                anim = ScrollAnimationCoroutine(endPos, startPos);
                while (anim.MoveNext())
                    yield return anim.Current;
            }
        }

        private IEnumerator ContinuousScrollAnimationCoroutine()
        {
            RectTransform rt = this.textComponent.rectTransform;
            float halfTextWidth = TextWidth / 2f;
            float halfWidth = rectTransform.rect.width / 2f;
            Vector2 startPos = new Vector2(halfTextWidth + halfWidth, 0f);
            Vector2 endPos = new Vector2(-halfTextWidth - halfWidth, 0f);

            while (true)
            {
                rt.anchoredPosition = startPos;
                yield return null;

                IEnumerator anim = ScrollAnimationCoroutine(startPos, endPos);
                while (anim.MoveNext())
                    yield return anim.Current;

                rt.anchoredPosition = endPos;
                yield return null;
            }
        }
        #endregion

        public enum ScrollMovementType
        {
            ByDuration,
            BySpeed
        }

        public enum ScrollAnimationType
        {
            Basic,
            FadeInOut,
            ForwardAndReverse,
            Continuous
        }
    }
}
