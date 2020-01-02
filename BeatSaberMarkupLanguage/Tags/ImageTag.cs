using BeatSaberMarkupLanguage.Components;
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

            ImageController image = gameObject.AddComponent<ImageController>();
            image.transform.SetParent(parent, false);

            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
