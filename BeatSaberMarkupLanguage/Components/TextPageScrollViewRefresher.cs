using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;

        private void OnEnable()
        {
            scrollView?.SetText(scrollView._text.text);
            scrollView?.RefreshButtons();
        }

        private void OnRectTransformDimensionsChange()
        {
            scrollView?.SetText(scrollView._text.text);
            scrollView?.RefreshButtons();
        }
    }
}
