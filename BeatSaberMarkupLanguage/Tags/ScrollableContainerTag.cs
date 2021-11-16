using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableContainerTag : BSMLTag
    {
        public override string[] Aliases { get; } = new[] { "scrollable-container" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = new GameObject("BSMLScrollScrollableContainer");
            go.SetActive(false);

            RectTransform transform = go.AddComponent<RectTransform>();
            transform.SetParent(parent, false);
            transform.localPosition = Vector2.zero;
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            GameObject vpgo = new GameObject("Viewport");
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

            GameObject contentgo = new GameObject("Content Wrapper");
            RectTransform content = contentgo.AddComponent<RectTransform>();
            content.SetParent(viewport, false);
            content.localPosition = Vector2.zero;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.pivot = new Vector2(0.5f, 1f);

            ContentSizeFitter contentFitter = contentgo.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            VerticalLayoutGroup layout = contentgo.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            /*LayoutElement layoutElement = contentgo.AddComponent<LayoutElement>();
            layoutElement.minWidth = -1;
            layoutElement.preferredWidth = -1;
            layoutElement.flexibleWidth = 0;*/

            go.AddComponent<Touchable>(); // Required by EventSystemListener
            go.AddComponent<EventSystemListener>(); // Required by ScrollView
            BSMLScrollableContainer scrollView = go.AddComponent<BSMLScrollableContainer>();
            scrollView.ContentRect = content;
            scrollView.Viewport = viewport;
            (scrollView as ScrollView).SetField("_platformHelper", BeatSaberUI.PlatformHelper);

            contentgo.AddComponent<ExternalComponents>().components.Add(scrollView);

            go.SetActive(true);
            return contentgo;
        }
    }
}