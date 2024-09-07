using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextPageScrollViewTag : BSMLTag
    {
        public override string[] Aliases => new[] { "text-page" };

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView scrollView = DiContainer.InstantiatePrefabForComponent<TextPageScrollView>(ScrollViewTag.ScrollViewTemplate, parent);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;

            TextMeshProUGUI textMesh = scrollView._text;
            textMesh.text = "Default Text";

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(textMesh.gameObject);

            textMesh.gameObject.AddComponent<TextPageScrollViewRefresher>().ScrollView = scrollView;

            List<Component> components = scrollView.gameObject.AddComponent<ExternalComponents>().Components;
            components.Add(textMesh);
            components.Add(localizedText);

            return scrollView.gameObject;
        }
    }
}
