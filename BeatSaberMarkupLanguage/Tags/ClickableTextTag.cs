using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ClickableTextTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "clickable-text" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLClickableText")
            {
                layer = 5,
            };

            gameObject.SetActive(false);

            ClickableText clickableText = gameObject.AddComponent<ClickableText>();
            clickableText.font = BeatSaberUI.MainTextFont;
            clickableText.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            clickableText.text = "Default Text";
            clickableText.fontSize = 5;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);

            CreateLocalizableText(gameObject);

            return new PrefabParams(gameObject);
        }
    }
}
