using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;
        public void Refresh()
        {
            scrollView?.SetText(scrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text").text);
        }
        void OnEnable()
        {
            Refresh();
        }
    }
}
