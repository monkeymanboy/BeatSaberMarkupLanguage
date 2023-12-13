using System.Collections;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;

        private Coroutine coroutine;

        private void OnEnable()
        {
            if (scrollView != null)
            {
                scrollView.SetText(scrollView._text.text);
                scrollView.RefreshButtons();
            }
        }

        private void OnRectTransformDimensionsChange()
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
