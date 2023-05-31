using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class RawImageTag : BSMLTag
    {
        public override string[] Aliases => new[] { "raw-image" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject("BSMLRawImage");

            RawImage rawImage = gameObject.AddComponent<RawImage>();
            rawImage.material = Utilities.ImageResources.NoGlowMat;
            rawImage.rectTransform.SetParent(parent, false);
            rawImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            rawImage.texture = Utilities.ImageResources.BlankSprite.texture;

            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
