using System;
using System.Collections;
using System.Threading.Tasks;
using IPA.Utilities.Async;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Animations
{
    [Obsolete]
    public enum AnimationType
    {
        GIF,
        APNG,
    }

    public class AnimationLoader
    {
        private static readonly int AtlasSizeLimit = Mathf.Min(SystemInfo.maxTextureSize, 4096);

        [Obsolete("Use ProcessApngAsync or ProcessGifAsync instead.")]
        public static void Process(AnimationType type, byte[] data, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                AnimationData animationData;

                switch (type)
                {
                    case AnimationType.GIF:
                        animationData = await ProcessGifAsync(data);
                        callback?.Invoke(animationData.Atlas, animationData.Uvs, animationData.Delays, animationData.Width, animationData.Height);
                        break;
                    case AnimationType.APNG:
                        animationData = await ProcessApngAsync(data);
                        callback?.Invoke(animationData.Atlas, animationData.Uvs, animationData.Delays, animationData.Width, animationData.Height);
                        break;
                }
            });
        }

        [Obsolete]
        public static IEnumerator ProcessAnimationInfo(AnimationInfo animationInfo, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            Texture2D[] textures = new Texture2D[animationInfo.Frames.Count];
            float[] delays = new float[animationInfo.Frames.Count];

            float lastThrottleTime = Time.realtimeSinceStartup;

            for (int i = 0; i < animationInfo.Frames.Count; i++)
            {
                if ((animationInfo.Frames?.Count ?? 0) <= i)
                {
                    yield return new WaitUntil(() => { return (animationInfo.Frames?.Count ?? 0) > i; });
                    lastThrottleTime = Time.realtimeSinceStartup;
                }

                FrameInfo currentFrameInfo = animationInfo.Frames[i];
                delays[i] = currentFrameInfo.Delay;

                Texture2D frameTexture = new(currentFrameInfo.Width, currentFrameInfo.Height, TextureFormat.BGRA32, false);
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                try
                {
                    frameTexture.LoadRawTextureData(currentFrameInfo.Colors);
                }
                catch
                {
                    yield break;
                }

                textures[i] = frameTexture;

                // Allow up to .5ms of thread usage for loading this anim
                if (Time.realtimeSinceStartup > lastThrottleTime + 0.0005f)
                {
                    yield return null;
                    lastThrottleTime = Time.realtimeSinceStartup;
                }
            }

            Texture2D atlasTexture = new(0, 0)
            {
                name = "AnimatedImageAtlas", // TODO: it'd be nice to have the actual image name here
            };

            Rect[] atlas = atlasTexture.PackTextures(textures, 2, AtlasSizeLimit, true);

            foreach (Texture2D texture in textures)
            {
                Object.Destroy(texture);
            }

            FrameInfo firstFrame = animationInfo.Frames[0];
            callback?.Invoke(atlasTexture, atlas, delays, firstFrame.Width, firstFrame.Height);
        }

        public static async Task<AnimationData> ProcessApngAsync(byte[] data)
        {
            AnimationInfo animationInfo = await APNGUnityDecoder.ProcessAsync(data);
            return await ProcessAnimationInfoAsync(animationInfo);
        }

        public static async Task<AnimationData> ProcessGifAsync(byte[] data)
        {
            AnimationInfo animationInfo = await GIFUnityDecoder.ProcessAsync(data);
            return await ProcessAnimationInfoAsync(animationInfo);
        }

        private static async Task<AnimationData> ProcessAnimationInfoAsync(AnimationInfo animationInfo)
        {
            Texture2D[] textures = new Texture2D[animationInfo.Frames.Count];
            float[] delays = new float[animationInfo.Frames.Count];

            float lastThrottleTime = Time.realtimeSinceStartup;

            for (int i = 0; i < animationInfo.Frames.Count; i++)
            {
                FrameInfo currentFrameInfo = animationInfo.Frames[i];
                delays[i] = currentFrameInfo.Delay;

                Texture2D frameTexture = new(currentFrameInfo.Width, currentFrameInfo.Height, TextureFormat.BGRA32, false);
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                frameTexture.LoadRawTextureData(currentFrameInfo.Colors);

                textures[i] = frameTexture;

                // Allow up to .5ms of thread usage for loading this anim
                if (Time.realtimeSinceStartup > lastThrottleTime + 0.0005f)
                {
                    await Task.Yield();
                    lastThrottleTime = Time.realtimeSinceStartup;
                }
            }

            Texture2D atlasTexture = new(0, 0)
            {
                name = "AnimatedImageAtlas", // TODO: it'd be nice to have the actual image name here
            };

            Rect[] atlas = atlasTexture.PackTextures(textures, 2, AtlasSizeLimit, true);

            foreach (Texture2D texture in textures)
            {
                Object.Destroy(texture);
            }

            FrameInfo firstFrame = animationInfo.Frames[0];
            return new AnimationData(atlasTexture, atlas, delays, firstFrame.Width, firstFrame.Height);
        }
    }
}
