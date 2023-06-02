using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;

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
            if (scrollView != null)
            {
                scrollView.SetText(scrollView._text.text);
                scrollView.RefreshButtons();
            }
        }
    }
}
