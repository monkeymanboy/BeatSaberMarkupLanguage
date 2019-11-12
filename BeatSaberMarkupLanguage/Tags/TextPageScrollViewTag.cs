using BS_Utils.Utilities;
using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextPageScrollViewTag : BSMLTag
    {
        public override string[] Aliases => new[] { "text-page" };

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView scrollView = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First().GetPrivateField<TextPageScrollView>("_textPageScrollView"), parent);
            scrollView.name = "BSMLTextPageScrollView";
            scrollView.enabled = true;
            return scrollView.gameObject;
        }
    }
}
