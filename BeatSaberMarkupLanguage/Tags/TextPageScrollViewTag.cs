using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            GameObject.Destroy(scrollView.transform.Find("Viewport").GetComponent<Canvas>());
            textMesh.gameObject.AddComponent<TextPageScrollViewRefresher>().scrollView = scrollView;
            scrollView.gameObject.AddComponent<ExternalComponents>().components.Add(textMesh);
            return scrollView.gameObject;
        }
    }
}
