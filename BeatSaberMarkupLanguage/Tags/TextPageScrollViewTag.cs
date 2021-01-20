using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextPageScrollViewTag : BSMLTag
    {
        public override string[] Aliases => new[] { "text-page" };

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView scrollView = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First().GetField<TextPageScrollView, ReleaseInfoViewController>("_textPageScrollView"), parent);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;

            TextMeshProUGUI textMesh = scrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text");
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
