using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ImageTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "image", "img" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLImage")
            {
                layer = 5,
            };

            Image image = gameObject.AddComponent<ImageView>();
            image.material = Utilities.ImageResources.NoGlowMat;
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.sprite = Utilities.ImageResources.BlankSprite;

            gameObject.AddComponent<LayoutElement>();
            return new PrefabParams(gameObject);
        }
    }
}
