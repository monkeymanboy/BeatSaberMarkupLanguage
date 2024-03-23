using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextPageScrollViewTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "text-page" };

        protected override PrefabParams CreatePrefab()
        {
            TextPageScrollView scrollView = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<TextPageScrollView>(ScrollViewTag.ScrollViewTemplate);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;

            TextMeshProUGUI textMesh = scrollView._text;
            textMesh.text = "Default Text";

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(textMesh.gameObject);

            textMesh.gameObject.AddComponent<TextPageScrollViewRefresher>().scrollView = scrollView;

            List<Component> components = scrollView.gameObject.AddComponent<ExternalComponents>().components;
            components.Add(textMesh);
            components.Add(localizedText);

            return new PrefabParams(scrollView.gameObject);
        }
    }
}
