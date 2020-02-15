using BeatSaberMarkupLanguage.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Image))]
    class ImageHandler : TypeHandler<Image>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "image", new[] { "source" , "src" } },
            { "preserveAspect", new[] { "preserve-aspect" } }
        };

        public override Dictionary<string, Action<Image, string>> Setters => new Dictionary<string, Action<Image, string>>()
        {
            { "image", new Action<Image, string>(SetImage) },
            { "preserveAspect", new Action<Image, string>((image, preserveAspect) => image.preserveAspect = bool.Parse(preserveAspect)) }

        };

        public void SetImage(Image image, string imagePath)
        {
            AnimationStateUpdater oldStateUpdater = image.GetComponent<AnimationStateUpdater>();
            if (oldStateUpdater != null)
                MonoBehaviour.Destroy(oldStateUpdater);

            if (imagePath.StartsWith("#"))
            {
                string imgName = imagePath.Substring(1);
                try
                {
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == imgName);
                }
                catch
                {
                    Logger.log.Error($"Could not find Texture with image name {imgName}");
                }
            }
            else if (imagePath.EndsWith(".gif") || imagePath.EndsWith(".apng"))
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.image = image;

                if (AnimationController.instance.IsRegistered(imagePath))
                {
                    stateUpdater.controllerData = AnimationController.instance.GetAnimationControllerData(imagePath);
                }
                else
                {
                    Utilities.GetData(imagePath, (byte[] data) => {
                        AnimationLoader.Process(imagePath.EndsWith(".gif") ? AnimationType.GIF : AnimationType.APNG, data, (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                        {
                            AnimationControllerData controllerData = AnimationController.instance.Register(imagePath, tex, uvs, delays);
                            stateUpdater.controllerData = controllerData;
                        });
                    });
                }
            }
            else
            {
                Utilities.GetData(imagePath, (byte[] data) =>
                {
                    image.sprite = Utilities.LoadSpriteRaw(data);
                    image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                });
            }
        }
    }
}
