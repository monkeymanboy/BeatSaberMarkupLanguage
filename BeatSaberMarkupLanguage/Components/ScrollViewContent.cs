using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class ScrollViewContent : MonoBehaviour
    {
        public ScrollView scrollView;

        void OnRectTransformDimensionsChange()
        {
            scrollView?.Setup();
            scrollView?.RefreshButtonsInteractibility();
        }
    }
}
