using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollViewTag : BSMLTag
    {
        public static TextPageScrollView ScrollViewTemplate { get; private set; }

        public override string[] Aliases => new[] { "scroll-view" };

        public override void Setup()
        {
            base.Setup();
            ScrollViewTemplate = DiContainer.Resolve<ReleaseInfoViewController>().GetComponentOnChild<TextPageScrollView>("TextPageScrollView");
        }

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView textScrollView = Object.Instantiate(ScrollViewTemplate, parent);
            textScrollView.name = "BSMLScrollView";
            Button pageUpButton = textScrollView._pageUpButton;
            Button pageDownButton = textScrollView._pageDownButton;
            VerticalScrollIndicator verticalScrollIndicator = textScrollView._verticalScrollIndicator;

            RectTransform viewport = textScrollView._viewport;
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(viewport.gameObject);

            Object.Destroy(textScrollView._text.gameObject);
            GameObject gameObject = textScrollView.gameObject;
            Object.Destroy(textScrollView);
            gameObject.SetActive(false);

            BSMLScrollView scrollView = DiContainer.InstantiateComponent<BSMLScrollView>(gameObject);
            scrollView._pageUpButton = pageUpButton;
            scrollView._pageDownButton = pageDownButton;
            scrollView._verticalScrollIndicator = verticalScrollIndicator;
            scrollView._viewport = viewport;

            viewport.anchorMin = new Vector2(0, 0);
            viewport.anchorMax = new Vector2(1, 1);

            GameObject parentObj = new("BSMLScrollViewContent")
            {
                layer = 5,
            };

            parentObj.transform.SetParent(viewport, false);

            ContentSizeFitter contentSizeFitter = parentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            VerticalLayoutGroup verticalLayout = parentObj.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childControlHeight = true;
            verticalLayout.childControlWidth = true;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;

            RectTransform rectTransform = parentObj.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 1);
            parentObj.AddComponent<ScrollViewContent>().scrollView = scrollView;

            GameObject child = new("BSMLScrollViewContentContainer")
            {
                layer = 5,
            };

            child.transform.SetParent(rectTransform, false);

            VerticalLayoutGroup layoutGroup = child.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.LowerCenter;
            layoutGroup.spacing = 0.5f;

            ExternalComponents externalComponents = child.AddComponent<ExternalComponents>();
            externalComponents.components.Add(scrollView);
            externalComponents.components.Add(scrollView.transform);
            externalComponents.components.Add(gameObject.AddComponent<LayoutElement>());

            (child.transform as RectTransform).sizeDelta = new Vector2(0, -1);

            scrollView._contentRectTransform = parentObj.transform as RectTransform;
            gameObject.SetActive(true);
            return child;
        }
    }
}
