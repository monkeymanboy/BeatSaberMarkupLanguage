using BS_Utils.Utilities;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    internal class TextPageScrollViewRefresher : MonoBehaviour
    {
        public TextPageScrollView scrollView;
        void OnEnable()
        {
            scrollView?.SetText(scrollView.GetPrivateField<TextMeshProUGUI>("_text").text);
        }
    }
}
