using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;
        void OnEnable()
        {
            scrollView?.SetText(scrollView._text.text);
            scrollView?.RefreshButtons();
        }

        void OnRectTransformDimensionsChange()
        {
            scrollView?.SetText(scrollView._text.text);
            scrollView?.RefreshButtons();
        }
    }
}
