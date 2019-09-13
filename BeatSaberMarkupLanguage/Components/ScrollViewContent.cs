using System.Collections;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class ScrollViewContent : MonoBehaviour
    {
        public ScrollView scrollView;

        private void Start()
        {
            StartCoroutine(SetupScrollView());
        }
        private IEnumerator SetupScrollView()
        {
            RectTransform rectTransform = (transform as RectTransform);
            yield return new WaitWhile(() => rectTransform.sizeDelta.y == -1); //This is a reliable way to wait for the vertical layout group to be set up which it must be before the scrollview can be setup
            scrollView.Setup();
            scrollView.RefreshButtonsInteractibility();
        }
    }
}
