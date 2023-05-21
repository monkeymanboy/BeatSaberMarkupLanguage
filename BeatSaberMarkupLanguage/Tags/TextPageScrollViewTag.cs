using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Collections.Generic;
using System.Linq;
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
                pageTemplate = Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First()._textPageScrollView;
            TextPageScrollView scrollView = diContainer.InstantiatePrefabForComponent<TextPageScrollView>(pageTemplate, parent);
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
