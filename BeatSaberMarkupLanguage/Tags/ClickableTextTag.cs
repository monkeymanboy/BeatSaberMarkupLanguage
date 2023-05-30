using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ClickableTextTag : BSMLTag
    {
        public override string[] Aliases => new[] { "clickable-text" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLClickableText";
            gameObject.SetActive(false);

            ClickableText clickableText = gameObject.AddComponent<ClickableText>();
            clickableText.font = BeatSaberUI.MainTextFont;
            clickableText.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            clickableText.rectTransform.SetParent(parent, false);
            clickableText.text = "Default Text";
            clickableText.fontSize = 5;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);

            CreateLocalizableText(gameObject);

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
