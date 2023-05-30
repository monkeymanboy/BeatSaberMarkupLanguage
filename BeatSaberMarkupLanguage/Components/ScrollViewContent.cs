using HMUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ScrollViewContent : MonoBehaviour
    {
        public ScrollView scrollView;
        private bool _dirty = false;

        private void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            StopAllCoroutines();
            StartCoroutine(SetupScrollView());
        }

        private void OnEnable()
        {
            UpdateScrollView();
        }

        private void OnRectTransformDimensionsChange()
        {
            _dirty = true; // Need to delay the update such that it doesn't run during the rebuild loop
        }

        private void Update()
        {
            if (_dirty)
            {
                _dirty = false;
                UpdateScrollView();
            }
        }

        private IEnumerator SetupScrollView()
        {
            RectTransform rectTransform = (transform.GetChild(0) as RectTransform);
            yield return new WaitWhile(() => rectTransform.sizeDelta.y == -1); //This is a reliable way to wait for the vertical layout group to be set up which it must be before the scrollview can be setup
            UpdateScrollView();
        }

        private void UpdateScrollView()
        {
            scrollView.SetContentSize((transform.GetChild(0) as RectTransform).rect.height);
            scrollView.RefreshButtons();
        }
    }
}
