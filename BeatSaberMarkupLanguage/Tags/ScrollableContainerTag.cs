using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableContainerTag : PrefabBSMLTag
    {
        public override string[] Aliases { get; } = new[] { "scrollable-container" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLScrollScrollableContainer")
            {
                layer = 5,
            };

            gameObject.SetActive(false);

            RectTransform transform = gameObject.AddComponent<RectTransform>();
            transform.localPosition = Vector2.zero;
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            GameObject vpgo = new("Viewport")
            {
                layer = 5,
            };

            RectTransform viewport = vpgo.AddComponent<RectTransform>();
            viewport.SetParent(transform, false);
            viewport.localPosition = Vector2.zero;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.anchoredPosition = Vector2.zero;
            viewport.sizeDelta = Vector2.zero;

            Mask vpmask = vpgo.AddComponent<Mask>();
            Image vpimage = vpgo.AddComponent<ImageView>(); // a Mask needs an Image to work
            vpmask.showMaskGraphic = false;
            vpimage.color = Color.white;
            vpimage.sprite = Utilities.ImageResources.WhitePixel;
            vpimage.material = Utilities.ImageResources.NoGlowMat;

            GameObject contentGameObject = new("Content Wrapper")
            {
                layer = 5,
            };

            RectTransform content = contentGameObject.AddComponent<RectTransform>();
            content.SetParent(viewport, false);
            content.localPosition = Vector2.zero;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.pivot = new Vector2(0.5f, 1f);

            ContentSizeFitter contentFitter = contentGameObject.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            VerticalLayoutGroup layout = contentGameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            gameObject.AddComponent<Touchable>(); // Required by EventSystemListener
            gameObject.AddComponent<EventSystemListener>(); // Required by ScrollView
            BSMLScrollableContainer scrollView = BeatSaberUI.DiContainer.InstantiateComponent<BSMLScrollableContainer>(gameObject);
            scrollView.ContentRect = content;
            scrollView.Viewport = viewport;

            contentGameObject.AddComponent<ExternalComponents>().components.Add(scrollView);

            return new PrefabParams(gameObject, contentGameObject);
        }
    }
}
