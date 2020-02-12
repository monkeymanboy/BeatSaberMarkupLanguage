using BeatSaberMarkupLanguage.Components;
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
            else if (imagePath.EndsWith(".gif"))
            {
                try
                {
                    Utilities.AssemblyFromPath(imagePath, out Assembly asm, out string newPath);
                    byte[] gifE = Utilities.GetResource(asm, newPath);

                    SharedCoroutineStarter.instance.StartCoroutine(Animations.GIF.AnimationDecoder.Process(gifE, (Texture2D tex, Rect[] uvs, float[] delays, bool consistent, int width, int height) =>
                    {
                        var t = Animations.AnimationController.Instance.Register(tex, uvs, delays);
                        image.sprite = t.sprite;
                        image.material = t.animMaterial;

                        //The ol' switcheroo!
                        image.preserveAspect = !image.preserveAspect;

                        t.IncRefs();
                    }));
                }
                catch
                {
                    Logger.log.Error($"Could not find GIF with name {imagePath} in resources!");
                }
            }
            else if ((imagePath.StartsWith("@") && imagePath.EndsWith(".png")) || imagePath.EndsWith(".apng"))
            {
                try
                {
                    string imgName = imagePath;
                    if (imagePath.StartsWith("@"))
                        imgName = imagePath.Substring(1);
                    Utilities.AssemblyFromPath(imgName, out Assembly asm, out string newPath);
                    byte[] apngE = Utilities.GetResource(asm, newPath);

                    
                    SharedCoroutineStarter.instance.StartCoroutine(Animations.APNGUnityDecoder.Process(Animations.APNG.FromStream(new System.IO.MemoryStream(apngE)), (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                    {
                        //oh god this wont work
                        var t = Animations.AnimationController.Instance.Register(tex, uvs, delays);
                        image.sprite = t.sprite;
                        image.material = t.animMaterial;

                        //The ol' switcheroo!
                        image.preserveAspect = !image.preserveAspect;

                        t.IncRefs();
                    }));
                }
                catch
                {
                    Logger.log.Error($"Could not find APNG with name {imagePath} in resources!");
                }
            }
            else
            {
                image.sprite = Utilities.FindSpriteInAssembly(imagePath);
            }
        }
    }
}
