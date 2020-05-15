using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;
        void OnEnable()
        {
            scrollView?.SetText(scrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text").text);
        }
    }
}
