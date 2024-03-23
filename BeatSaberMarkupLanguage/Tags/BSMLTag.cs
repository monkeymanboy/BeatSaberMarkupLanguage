using System;
using BeatSaberMarkupLanguage.Util;
using BGLib.Polyglot;
using TMPro;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class BSMLTag
    {
        [Obsolete]
        public bool isInitialized = false;

        public abstract string[] Aliases { get; }

        public virtual bool AddChildren { get => true; }

        [Obsolete("Use BeatSaberUI.DiContainer instead")]
        protected DiContainer DiContainer => BeatSaberUI.DiContainer;

        public abstract GameObject CreateObject(Transform parent);

        [Obsolete("This method is only called once in the entire lifetime of the application. Please use SetUp instead, which is called on internal restarts as well.")]
        public virtual void Setup()
        {
        }

        public virtual void SetUp()
        {
        }

        protected LocalizedTextMeshProUGUI CreateLocalizableText(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out TextMeshProUGUI textMesh))
            {
                throw new InvalidOperationException($"{nameof(CreateLocalizableText)} should only be called if a {nameof(TextMeshProUGUI)} instance already exists on the {nameof(GameObject)}");
            }

            bool wasActive = gameObject.activeSelf;
            gameObject.SetActive(false);

            LocalizedTextMeshProUGUI localizedText = gameObject.AddComponent<LocalizedTextMeshProUGUI>();

            localizedText.enabled = false;
            localizedText.localizedComponent = textMesh;

            gameObject.SetActive(wasActive);

            return localizedText;
        }

        protected LocalizedTextMeshProUGUI ConfigureLocalizedText(GameObject gameObject)
        {
            LocalizedTextMeshProUGUI localizedText = gameObject.GetComponent<LocalizedTextMeshProUGUI>();

            localizedText.enabled = false;
            localizedText.key = string.Empty;
            Localization.Instance.RemoveOnLocalizeEvent(localizedText);

            return localizedText;
        }
    }
}
