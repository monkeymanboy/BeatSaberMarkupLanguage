using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollingTextTag : BSMLTag
    {
        public override string[] Aliases => new[] { "scrolling-text" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject("BSMLScrollingText");
            gameObject.transform.SetParent(parent, false);

            ScrollingText scrollingText = gameObject.AddComponent<ScrollingText>();
            scrollingText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            scrollingText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            scrollingText.rectTransform.sizeDelta = new Vector2(90f, 6f);
            scrollingText.textComponent.text = "Default Text";
            scrollingText.textComponent.fontSize = 3f;
            scrollingText.textComponent.color = Color.white;

            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 6f;
            layoutElement.preferredWidth = 90f;

            ExternalComponents externalComponents = gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(scrollingText.textComponent);

            return gameObject;
        }
    }
}
