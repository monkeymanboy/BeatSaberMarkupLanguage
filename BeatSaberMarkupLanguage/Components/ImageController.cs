using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    class ImageController : MonoBehaviour
    {
        private bool useRawImage;
        private Image image;
        private RawImage rawImage;

        public void SpawnImageComponent(bool raw)
        {
            useRawImage = raw;

            if (useRawImage)
            {
                rawImage = gameObject.AddComponent<RawImage>();
                rawImage.material = Utilities.ImageResources.NoGlowMat;
                rawImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
                rawImage.texture = Utilities.ImageResources.BlankSprite.texture;
            }
            else
            {
                image = gameObject.AddComponent<Image>(); 
                image.material = Utilities.ImageResources.NoGlowMat;
                image.rectTransform.sizeDelta = new Vector2(20f, 20f);
                image.sprite = Utilities.ImageResources.BlankSprite;
            }
        }

        public void SetImage(string imagePath)
        {
            if (useRawImage)
            {
                if (imagePath.StartsWith("#"))
                {
                    var imgName = imagePath.Substring(1);
                    rawImage.texture = Resources.FindObjectsOfTypeAll<Texture>().First(x => x.name == imgName);
                }
                else
                {
                    rawImage.texture = Utilities.FindTextureInAssembly(imagePath);
                }
            }
            else
            {
                if (imagePath.StartsWith("#"))
                {
                    var imgName = imagePath.Substring(1);
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == imgName);
                }
                else
                {
                    image.sprite = Utilities.FindSpriteInAssembly(imagePath);
                }
            }
        }

        public void SetPreserveAspect(bool preserveAspect)
        {
            if (!useRawImage)
            {
                image.preserveAspect = preserveAspect;
            }
        }

    }
}
