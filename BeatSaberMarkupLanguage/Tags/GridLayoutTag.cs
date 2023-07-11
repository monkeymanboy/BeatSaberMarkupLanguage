using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class GridLayoutTag : BSMLTag
    {
        public override string[] Aliases => new[] { "grid" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new("BSMLGridLayoutGroup")
            {
                layer = 5,
            };

            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<GridLayoutGroup>();
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return gameObject;
        }
    }
}
