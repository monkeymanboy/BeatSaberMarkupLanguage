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
            layoutElement.preferredWidth = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;

            ContentSizeFitter sizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform buttonTransform = button.transform.GetChild(0) as RectTransform;
            buttonTransform.anchorMin = new Vector2(0, 0);
            buttonTransform.anchorMax = new Vector2(1, 1);
            buttonTransform.sizeDelta = new Vector2(0, 0);

            (button.transform as RectTransform).pivot = new Vector2(.5f, .5f);
            RectTransform glow = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), button.transform).transform as RectTransform;
            glow.gameObject.name = "BSMLPageButtonGlowContainer";
            glow.SetParent(buttonTransform);
            glow.anchorMin = new Vector2(0, 0);
            glow.anchorMax = new Vector2(1, 1);
            glow.sizeDelta = new Vector2(0, 0);
            glow.anchoredPosition = new Vector2(0, 0);


            button.gameObject.SetActive(true);
            return button.gameObject;
        }
    }
}
