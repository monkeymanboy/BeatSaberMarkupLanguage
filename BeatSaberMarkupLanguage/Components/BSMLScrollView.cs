using System.Collections;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLScrollView : ScrollView
    {
        private Coroutine updateLayoutCoroutine;

        internal void UpdateViewport()
        {
            // Resize viewport so it doesn't overlap with scroll bar
            viewportTransform.offsetMax = _verticalScrollIndicator != null && _verticalScrollIndicator.gameObject.activeSelf ? new Vector2(-6, 0) : Vector3.zero;
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled && updateLayoutCoroutine == null)
            {
                updateLayoutCoroutine = StartCoroutine(UpdateLayoutCoroutine());
            }
        }

        private IEnumerator UpdateLayoutCoroutine()
        {
            yield return null;
            UpdateContentSize();
            RefreshButtons();
            updateLayoutCoroutine = null;
        }
    }
}
