using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ImageTag : BSMLTag
    {
        public override string[] Aliases => new[] { "image", "img" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new("BSMLImage")
            {
                layer = 5,
            };

            Image image = gameObject.AddComponent<ImageView>();
            image.material = Utilities.ImageResources.NoGlowMat;
            image.rectTransform.SetParent(parent, false);
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.sprite = Utilities.ImageResources.BlankSprite;

            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
