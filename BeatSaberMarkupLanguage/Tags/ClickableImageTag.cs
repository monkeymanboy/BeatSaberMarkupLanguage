using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ClickableImageTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "clickable-image" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLClickableImage")
            {
                layer = 5,
            };

            Image image = gameObject.AddComponent<ClickableImage>();
            image.material = Utilities.ImageResources.NoGlowMat;
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.sprite = Utilities.ImageResources.BlankSprite;

            gameObject.AddComponent<LayoutElement>();
            return new PrefabParams(gameObject);
        }
    }
}
