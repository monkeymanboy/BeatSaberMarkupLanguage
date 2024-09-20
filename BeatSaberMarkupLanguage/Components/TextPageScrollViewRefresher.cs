using System.Collections;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        [SerializeField]
        private TextPageScrollView scrollView;

        private Coroutine coroutine;

        public TextPageScrollView ScrollView
        {
            get => scrollView;
            set => scrollView = value;
        }

        protected void OnEnable()
        {
            if (scrollView != null)
            {
                scrollView.SetText(scrollView._text.text);
                scrollView.RefreshButtons();
            }
        }

        protected void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled && scrollView != null && coroutine == null)
            {
                // SetText can eventually enable/disable GameObjects which isn't allowed at this point (UI rebuild) so we delay our update
                coroutine = StartCoroutine(UpdateLayoutCoroutine());
            }
        }

        private IEnumerator UpdateLayoutCoroutine()
        {
            yield return null;
            scrollView.SetText(scrollView._text.text);
            scrollView.RefreshButtons();
            coroutine = null;
        }
    }
}
