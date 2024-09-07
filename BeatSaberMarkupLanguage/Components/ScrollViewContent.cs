using System.Collections;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ScrollViewContent : MonoBehaviour
    {
        public ScrollView ScrollView;
        private bool dirty = false;

        protected void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            StopAllCoroutines();
            StartCoroutine(SetupScrollView());
        }

        protected void OnEnable()
        {
            UpdateScrollView();
        }

        protected void OnRectTransformDimensionsChange()
        {
            dirty = true; // Need to delay the update such that it doesn't run during the rebuild loop
        }

        protected void Update()
        {
            if (dirty)
            {
                dirty = false;
                UpdateScrollView();
            }
        }

        private IEnumerator SetupScrollView()
        {
            RectTransform rectTransform = (RectTransform)transform.GetChild(0);
            yield return new WaitWhile(() => rectTransform.sizeDelta.y == -1); // This is a reliable way to wait for the vertical layout group to be set up which it must be before the scrollview can be setup
            UpdateScrollView();
        }

        private void UpdateScrollView()
        {
            ScrollView.SetContentSize((transform.GetChild(0) as RectTransform).rect.height);
            ScrollView.RefreshButtons();
        }
    }
}
