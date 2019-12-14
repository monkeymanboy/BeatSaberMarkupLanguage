using BeatSaberMarkupLanguage.Components;
using BS_Utils.Utilities;
using HMUI;
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
            TextPageScrollView textScrollView = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First().GetPrivateField<TextPageScrollView>("_textPageScrollView"), parent);
            textScrollView.name = "BSMLScrollableSettingsContainer";
            Button pageUpButton = textScrollView.GetPrivateField<Button>("_pageUpButton");
            Button pageDownButton = textScrollView.GetPrivateField<Button>("_pageDownButton");
            VerticalScrollIndicator verticalScrollIndicator = textScrollView.GetPrivateField<VerticalScrollIndicator>("_verticalScrollIndicator");
            RectTransform viewport = textScrollView.GetPrivateField<RectTransform>("_viewport");
            GameObject.Destroy(textScrollView.GetPrivateField<TextMeshProUGUI>("_text").gameObject);
            GameObject gameObject = textScrollView.gameObject;
            MonoBehaviour.Destroy(textScrollView);
            gameObject.SetActive(false);

            BSMLScrollView scrollView = gameObject.AddComponent<BSMLScrollView>();
            scrollView.SetPrivateField("_pageUpButton", pageUpButton);
            scrollView.SetPrivateField("_pageDownButton", pageDownButton);
            scrollView.SetPrivateField("_verticalScrollIndicator", verticalScrollIndicator);
            scrollView.SetPrivateField("_viewport", viewport);

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

            scrollView.SetPrivateField("_contentRectTransform", parentObj.transform as RectTransform);
            gameObject.SetActive(true);
            return child;
        }
    }
}
