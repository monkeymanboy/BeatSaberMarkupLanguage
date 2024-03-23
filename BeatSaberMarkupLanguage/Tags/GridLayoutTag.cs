using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class GridLayoutTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "grid" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLGridLayoutGroup")
            {
                layer = 5,
            };

            gameObject.AddComponent<GridLayoutGroup>();
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return new PrefabParams(gameObject);
        }
    }
}
