using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollIndicatorTag : PrefabBSMLTag
    {
        public static GameObject VerticalScrollTemplate => ScrollViewTag.ScrollViewTemplate._verticalScrollIndicator.gameObject;

        public override string[] Aliases { get; } = new[] { "vertical-scroll-indicator", "scroll-indicator" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = Object.Instantiate(VerticalScrollTemplate);
            gameObject.SetActive(false);
            gameObject.name = "BSMLScrollIndicator";

            RectTransform transform = (RectTransform)gameObject.transform;

            Object.DestroyImmediate(gameObject.GetComponent<VerticalScrollIndicator>());
            BSMLScrollIndicator indicator = gameObject.AddComponent<BSMLScrollIndicator>();
            indicator.Handle = (RectTransform)transform.GetChild(0);

            return new PrefabParams(gameObject);
        }
    }
}
