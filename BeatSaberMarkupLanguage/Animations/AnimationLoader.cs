using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Animations
{
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
            switch (type)
            {
                case AnimationType.GIF:
                    BeatSaberUI.CoroutineStarter.StartCoroutine(GIFUnityDecoder.Process(data, (AnimationInfo animationInfo) => BeatSaberUI.CoroutineStarter.StartCoroutine(ProcessAnimationInfo(animationInfo, callback))));
                    break;
                case AnimationType.APNG:
                    BeatSaberUI.CoroutineStarter.StartCoroutine(APNGUnityDecoder.Process(data, (AnimationInfo animationInfo) => BeatSaberUI.CoroutineStarter.StartCoroutine(ProcessAnimationInfo(animationInfo, callback))));
                    break;
            }
        }

        [Obsolete]
        public static IEnumerator ProcessAnimationInfo(AnimationInfo animationInfo, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            int textureSize = AtlasSizeLimit, width = 0, height = 0;
            Texture2D texture = null;
            Texture2D[] texList = new Texture2D[animationInfo.frames.Count];
            float[] delays = new float[animationInfo.frames.Count];

            float lastThrottleTime = Time.realtimeSinceStartup;

            for (int i = 0; i < animationInfo.frames.Count; i++)
            {
                if ((animationInfo.frames?.Count ?? 0) <= i)
                {
                    yield return new WaitUntil(() => { return (animationInfo.frames?.Count ?? 0) > i; });
                    lastThrottleTime = Time.realtimeSinceStartup;
                }

                if (texture == null)
                {
                    textureSize = GetTextureSize(animationInfo, i);
                    texture = new Texture2D(animationInfo.frames[i].width, animationInfo.frames[i].height);

                    width = animationInfo.frames[i].width;
                    height = animationInfo.frames[i].height;
                }

                FrameInfo currentFrameInfo = animationInfo.frames[i];
                delays[i] = currentFrameInfo.delay;

                Texture2D frameTexture = new(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.BGRA32, false);
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                try
                {
                    frameTexture.LoadRawTextureData(currentFrameInfo.colors);
                }
                catch
                {
                    yield break;
                }

                texList[i] = frameTexture;

                // Allow up to .5ms of thread usage for loading this anim
                if (Time.realtimeSinceStartup > lastThrottleTime + 0.0005f)
                {
                    yield return null;
                    lastThrottleTime = Time.realtimeSinceStartup;
                }
            }

            Rect[] atlas = texture.PackTextures(texList, 2, textureSize, true);
            foreach (Texture2D frameTex in texList)
            {
                Object.Destroy(frameTex);
            }

            callback?.Invoke(texture, atlas, delays, width, height);
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
            int textureSize = AtlasSizeLimit;
            int width = 0;
            int height = 0;
            Texture2D texture = null;
            Texture2D[] texList = new Texture2D[animationInfo.frames.Count];
            float[] delays = new float[animationInfo.frames.Count];

            float lastThrottleTime = Time.realtimeSinceStartup;

            for (int i = 0; i < animationInfo.frames.Count; i++)
            {
                if (texture == null)
                {
                    textureSize = GetTextureSize(animationInfo, i);
                    texture = new Texture2D(animationInfo.frames[i].width, animationInfo.frames[i].height);

                    width = animationInfo.frames[i].width;
                    height = animationInfo.frames[i].height;
                }

                FrameInfo currentFrameInfo = animationInfo.frames[i];
                delays[i] = currentFrameInfo.delay;

                Texture2D frameTexture = new(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.BGRA32, false);
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                frameTexture.LoadRawTextureData(currentFrameInfo.colors);

                texList[i] = frameTexture;

                // Allow up to .5ms of thread usage for loading this anim
                if (Time.realtimeSinceStartup > lastThrottleTime + 0.0005f)
                {
                    await Task.Yield();
                    lastThrottleTime = Time.realtimeSinceStartup;
                }
            }

            Rect[] atlas = texture.PackTextures(texList, 2, textureSize, true);
            foreach (Texture2D frameTex in texList)
            {
                Object.Destroy(frameTex);
            }

            return new AnimationData(texture, atlas, delays, width, height);
        }

        private static int GetTextureSize(AnimationInfo frameInfo, int i)
        {
            int testNum = 2;
            int numFramesInRow;
            int numFramesInColumn;
            while (true)
            {
                int numFrames = frameInfo.frames.Count;

                // Make sure the number of frames is cleanly divisible by our testNum
                if (numFrames % testNum == 0)
                {
                    numFrames += numFrames % testNum;
                }

                // Math.Max to ensure numFramesInRow never becomes 0 to prevent DivideByZeroException
                // This would happen with single frame GIFs
                numFramesInRow = Math.Max(numFrames / testNum, 1);
                numFramesInColumn = numFrames / numFramesInRow;

                if (numFramesInRow <= numFramesInColumn)
                {
                    break;
                }

                testNum += 2;
            }

            int textureWidth = Mathf.Clamp(numFramesInRow * frameInfo.frames[i].width, 0, AtlasSizeLimit);
            int textureHeight = Mathf.Clamp(numFramesInColumn * frameInfo.frames[i].height, 0, AtlasSizeLimit);
            return Mathf.Max(textureWidth, textureHeight);
        }
    }
}
