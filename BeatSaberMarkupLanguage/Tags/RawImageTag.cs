using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class RawImageTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "raw-image" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLRawImage")
            {
                layer = 5,
            };

            RawImage rawImage = gameObject.AddComponent<RawImage>();
            rawImage.material = Utilities.ImageResources.NoGlowMat;
            rawImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            rawImage.texture = Utilities.ImageResources.BlankSprite.texture;

            gameObject.AddComponent<LayoutElement>();
            return new PrefabParams(gameObject);
        }
    }
}
