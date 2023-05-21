using BeatSaberMarkupLanguage.Components;
using Polyglot;
using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class BSMLTag
    {
        protected DiContainer diContainer => BeatSaberUI.DiContainer;

        public bool isInitialized = false;
        public abstract string[] Aliases { get; }
        public virtual bool AddChildren { get => true; }

        public abstract GameObject CreateObject(Transform parent);
        public virtual void Setup() { }

        protected LocalizableText CreateLocalizableText(GameObject gameObject)
        {
            TextMeshProUGUI textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (!textMesh) throw new InvalidOperationException("CreateLocalizableText should only be called if a TextMeshProUGUI instance already exists on the GameObject");

            bool wasActive = gameObject.activeSelf;
            gameObject.SetActive(false);

            LocalizableText localizableText = gameObject.AddComponent<LocalizableText>();

            localizableText.enabled = false;
            localizableText.TextMesh = gameObject.GetComponent<TextMeshProUGUI>();

            gameObject.SetActive(wasActive);

            return localizableText;
        }

        protected LocalizedTextMeshProUGUI ConfigureLocalizedText(GameObject gameObject)
        {
            LocalizedTextMeshProUGUI localizedText = gameObject.GetComponent<LocalizedTextMeshProUGUI>();

            localizedText.enabled = false;
            localizedText.Key = string.Empty;
            Localization.Instance.RemoveOnLocalizeEvent(localizedText);

            return localizedText;
        }
    }
}
