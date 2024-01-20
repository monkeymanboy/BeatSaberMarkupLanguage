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
        public bool isInitialized = false;

        public abstract string[] Aliases { get; }

        public virtual bool AddChildren { get => true; }

        protected DiContainer DiContainer => BeatSaberUI.DiContainer;

        public abstract GameObject CreateObject(Transform parent);

        public virtual void Setup()
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
