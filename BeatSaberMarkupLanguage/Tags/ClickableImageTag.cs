using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ClickableImageTag : BSMLTag
    {
        public override string[] Aliases => new[] { "clickable-image" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject("BSMLClickableImage");

            Image image = gameObject.AddComponent<ClickableImage>();
            image.material = Utilities.ImageResources.NoGlowMat;
            image.rectTransform.SetParent(parent, false);
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.sprite = Utilities.ImageResources.BlankSprite;

            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
