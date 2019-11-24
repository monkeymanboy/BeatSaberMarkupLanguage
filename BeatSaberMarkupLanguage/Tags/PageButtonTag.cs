using BeatSaberMarkupLanguage.Components;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PageButtonTag : BSMLTag
    {
        public override string[] Aliases => new[] { "page-button", "pg-button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PageUpButton")), parent, false);
            button.gameObject.SetActive(false);
            button.name = "BSMLPageButton";
            button.interactable = true;
            button.gameObject.AddComponent<PageButton>();
            LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 40;
            layoutElement.minHeight = 6;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;

            ContentSizeFitter sizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            RectTransform buttonTransform = button.transform.GetChild(0) as RectTransform;
            (button.transform as RectTransform).pivot = new Vector2(.5f, .5f);
            RectTransform glow = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), button.transform).transform as RectTransform;
            glow.gameObject.name = "BSMLPageButtonGlowContainer";
            glow.SetParent(buttonTransform);
            glow.localPosition = buttonTransform.localPosition;
            glow.anchoredPosition = buttonTransform.anchoredPosition;
            glow.anchorMin = buttonTransform.anchorMin;
            glow.anchorMax = buttonTransform.anchorMax;
            glow.sizeDelta = buttonTransform.sizeDelta;


            button.gameObject.SetActive(true);
            return button.gameObject;
        }
    }
}
