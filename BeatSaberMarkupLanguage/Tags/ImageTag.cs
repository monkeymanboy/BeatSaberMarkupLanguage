using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ImageTag : BSMLTag
    {
        public override string[] Aliases => new[] { "image" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject("BSMLImage");

            RawImage image = gameObject.AddComponent<RawImage>();
            image.material = Utilities.ImageResources.NoGlowMat;
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.rectTransform.SetParent(parent, false);
            image.texture = Utilities.ImageResources.BlankSprite.texture;

            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
