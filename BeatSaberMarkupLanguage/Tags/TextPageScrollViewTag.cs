using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using TMPro;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextPageScrollViewTag : BSMLTag
    {
        private TextPageScrollView pageTemplate;

        public override string[] Aliases => new[] { "text-page" };

        public override void Setup()
        {
            base.Setup();
            pageTemplate = DiContainer.Resolve<ReleaseInfoViewController>().GetComponentOnChild<TextPageScrollView>("TextPageScrollView");
        }

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView scrollView = DiContainer.InstantiatePrefabForComponent<TextPageScrollView>(pageTemplate, parent);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;

            TextMeshProUGUI textMesh = scrollView._text;
            textMesh.text = "Default Text";

            LocalizableText localizedText = CreateLocalizableText(textMesh.gameObject);

            textMesh.gameObject.AddComponent<TextPageScrollViewRefresher>().scrollView = scrollView;

            List<Component> components = scrollView.gameObject.AddComponent<ExternalComponents>().components;
            components.Add(textMesh);
            components.Add(localizedText);

            return scrollView.gameObject;
        }
    }
}
