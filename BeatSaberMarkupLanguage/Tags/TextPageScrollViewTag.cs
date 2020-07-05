using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
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
            TextPageScrollViewRefresher refresher = scrollView.gameObject.AddComponent<TextPageScrollViewRefresher>();
            refresher.scrollView = scrollView;
            TextMeshProProxy proxy = scrollView.gameObject.AddComponent<TextMeshProProxy>();
            proxy.Text =  scrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text");
            proxy.TextPageScrollViewRefresher = refresher;
            return scrollView.gameObject;
        }
    }
}
