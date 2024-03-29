using System.Collections;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLScrollView : ScrollView
    {
        private Coroutine updateLayoutCoroutine;

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
