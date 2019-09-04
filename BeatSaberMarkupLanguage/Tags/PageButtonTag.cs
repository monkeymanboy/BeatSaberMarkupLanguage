using BeatSaberMarkupLanguage.Components;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            layoutElement.preferredWidth = 40;
            layoutElement.preferredHeight = 6;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;
            ContentSizeFitter sizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform buttonTransform = button.transform.GetChild(0) as RectTransform;
            RectTransform glow = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Last(x => (x.name == "GlowContainer")), button.transform).transform as RectTransform;
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
