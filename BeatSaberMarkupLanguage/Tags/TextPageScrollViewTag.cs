using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
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
            scrollView.gameObject.AddComponent<TextPageScrollViewRefresher>().scrollView = scrollView;
            return scrollView.gameObject;
        }
    }
}
