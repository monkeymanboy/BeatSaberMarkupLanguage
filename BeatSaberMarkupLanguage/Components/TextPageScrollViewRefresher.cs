using System.Collections;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView ScrollView;

        private Coroutine coroutine;

        private void OnEnable()
        {
            if (ScrollView != null)
            {
                ScrollView.SetText(ScrollView._text.text);
                ScrollView.RefreshButtons();
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled && ScrollView != null && coroutine == null)
            {
                // SetText can eventually enable/disable GameObjects which isn't allowed at this point (UI rebuild) so we delay our update
                coroutine = StartCoroutine(UpdateLayoutCoroutine());
            }
        }

        private IEnumerator UpdateLayoutCoroutine()
        {
            yield return null;
            ScrollView.SetText(ScrollView._text.text);
            ScrollView.RefreshButtons();
            coroutine = null;
        }
    }
}
