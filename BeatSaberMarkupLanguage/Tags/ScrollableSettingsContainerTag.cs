using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableSettingsContainerTag : BSMLTag
    {
        public override string[] Aliases => new[] { "scrollable-settings-container" };

        public override GameObject CreateObject(Transform parent)
        {
            TextPageScrollView textScrollView = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First().GetField<TextPageScrollView, ReleaseInfoViewController>("_textPageScrollView"), parent);
            textScrollView.name = "BSMLScrollableSettingsContainer";
            Button pageUpButton = textScrollView.GetField<Button, TextPageScrollView>("_pageUpButton");
            Button pageDownButton = textScrollView.GetField<Button, TextPageScrollView>("_pageDownButton");
            VerticalScrollIndicator verticalScrollIndicator = textScrollView.GetField<VerticalScrollIndicator, TextPageScrollView>("_verticalScrollIndicator");
            RectTransform viewport = textScrollView.GetField<RectTransform, TextPageScrollView>("_viewport");
            GameObject.Destroy(textScrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text").gameObject);
            GameObject gameObject = textScrollView.gameObject;
            MonoBehaviour.Destroy(textScrollView);
            gameObject.SetActive(false);

            BSMLScrollView scrollView = gameObject.AddComponent<BSMLScrollView>();
            scrollView.SetField<ScrollView, Button>("_pageUpButton", pageUpButton);
            scrollView.SetField<ScrollView, Button>("_pageDownButton", pageDownButton);
            scrollView.SetField<ScrollView, VerticalScrollIndicator>("_verticalScrollIndicator", verticalScrollIndicator);
            scrollView.SetField<ScrollView, RectTransform>("_viewport", viewport);

            RectTransform scrollTransform = scrollView.transform as RectTransform;
            scrollTransform.anchoredPosition = new Vector2(2, 6);
            scrollTransform.sizeDelta = new Vector2(0, -20);

            viewport.anchorMin = new Vector2(0.5f, 0.5f);
            viewport.anchorMax = new Vector2(0.5f, 0.5f);
            viewport.sizeDelta = new Vector2(90, 40);
            
            GameObject parentObj = new GameObject();
            parentObj.name = "BSMLScrollableSettingsContent";
            parentObj.transform.SetParent(viewport, false);

            VerticalLayoutGroup verticalLayout = parentObj.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;

            RectTransform rectTransform = parentObj.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);
            parentObj.AddComponent<LayoutElement>();

            GameObject child = new GameObject();
            child.name = "BSMLScrollableSettingsContentContainer";
            child.transform.SetParent(rectTransform, false);

            VerticalLayoutGroup layoutGroup = child.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.spacing = 0.5f;

            child.AddComponent<ContentSizeFitter>();
            child.AddComponent<LayoutElement>();
            child.AddComponent<ScrollViewContent>().scrollView = scrollView;

            (child.transform as RectTransform).sizeDelta = new Vector2(0, -1);

            scrollView.SetField<ScrollView, RectTransform>("_contentRectTransform", parentObj.transform as RectTransform);
            gameObject.SetActive(true);
            return child;
        }
    }
}
