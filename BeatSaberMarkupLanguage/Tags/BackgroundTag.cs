using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class BackgroundTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "bg", "background", "div" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLBackground");
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();

            RectTransform rectTransform = (RectTransform)gameObject.transform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return new PrefabParams(gameObject);
        }
    }
}
