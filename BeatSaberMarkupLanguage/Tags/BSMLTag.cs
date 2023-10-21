﻿using System;
using BeatSaberMarkupLanguage.Components;
using Polyglot;
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

        // the idea here is to eventually provide the DiContainer instance through the constructor
        protected DiContainer DiContainer => BeatSaberUI.DiContainer;

        public abstract GameObject CreateObject(Transform parent);

        public virtual void Setup()
        {
        }

        internal GameObject CreateObjectInternal(Transform parent)
        {
            return CreateObject(parent);
        }

        protected LocalizableText CreateLocalizableText(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out TextMeshProUGUI textMesh))
            {
                throw new InvalidOperationException($"{nameof(CreateLocalizableText)} should only be called if a {nameof(TextMeshProUGUI)} instance already exists on the {nameof(GameObject)}");
            }

            bool wasActive = gameObject.activeSelf;
            gameObject.SetActive(false);

            LocalizableText localizableText = gameObject.AddComponent<LocalizableText>();

            localizableText.enabled = false;
            localizableText.TextMesh = textMesh;

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
