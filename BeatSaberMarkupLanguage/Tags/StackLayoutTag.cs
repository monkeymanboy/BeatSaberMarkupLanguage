using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class StackLayoutTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "stack" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLStackLayoutGroup")
            {
                layer = 5,
            };

            gameObject.AddComponent<StackLayoutGroup>();
            gameObject.AddComponent<ContentSizeFitter>();
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
