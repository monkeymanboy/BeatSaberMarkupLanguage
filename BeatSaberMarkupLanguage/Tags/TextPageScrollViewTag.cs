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
        private TextPageScrollView pageTemplate;

        public override string[] Aliases => new[] { "text-page" };

        public override GameObject CreateObject(Transform parent)
        {
            if (pageTemplate == null)
            {
                pageTemplate = Object.FindObjectOfType<EulaDisplayViewController>(true)._textPageScrollView;
            }

            TextPageScrollView scrollView = DiContainer.InstantiatePrefabForComponent<TextPageScrollView>(pageTemplate, parent);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;

            TextMeshProUGUI textMesh = scrollView._text;
            textMesh.text = "Default Text";

            LocalizedTextMeshProUGUI localizedText = CreateLocalizableText(textMesh.gameObject);

            textMesh.gameObject.AddComponent<TextPageScrollViewRefresher>().scrollView = scrollView;

            List<Component> components = scrollView.gameObject.AddComponent<ExternalComponents>().components;
            components.Add(textMesh);
            components.Add(localizedText);

            return scrollView.gameObject;
        }
    }
}
