using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class VerticalLayoutTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "vertical" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLVerticalLayoutGroup");
            gameObject.AddComponent<VerticalLayoutGroup>();

            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.AddComponent<Backgroundable>();

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            gameObject.AddComponent<LayoutElement>();

            return new PrefabParams(gameObject);
        }
    }
}
