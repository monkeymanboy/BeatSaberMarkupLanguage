using System;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace BeatSaberMarkupLanguage.Animations
{
    public enum AnimationType
    {
        GIF, APNG
    }
    public class AnimationLoader
    {
        public static void Process(AnimationType type, byte[] data, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            switch (type)
            {
                case AnimationType.GIF:
                    SharedCoroutineStarter.instance.StartCoroutine(GIFUnityDecoder.Process(data, (AnimationInfo animationInfo) => SharedCoroutineStarter.instance.StartCoroutine(ProcessAnimationInfo(animationInfo, callback))));
                    break;
                case AnimationType.APNG:
                    SharedCoroutineStarter.instance.StartCoroutine(APNGUnityDecoder.Process(data, (AnimationInfo animationInfo) => SharedCoroutineStarter.instance.StartCoroutine(ProcessAnimationInfo(animationInfo, callback))));
                    break;
            }
        }

        public static IEnumerator ProcessAnimationInfo(AnimationInfo animationInfo, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            int textureSize = SystemInfo.maxTextureSize >= 4096 ? 4096 : 2048, width = 0, height = 0;
            Texture2D texture = null;
            Texture2D[] texList = new Texture2D[animationInfo.frameCount];
            float[] delays = new float[animationInfo.frameCount];

            var lastThrottleTime = Time.realtimeSinceStartup;

            for (int i = 0; i < animationInfo.frameCount; i++)
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

                Texture2D frameTexture = new Texture2D(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.BGRA32, false);
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
                if(Time.realtimeSinceStartup > lastThrottleTime + 0.0005f) {
                    yield return null;
                    lastThrottleTime = Time.realtimeSinceStartup;
                }
            }

            Rect[] atlas = texture.PackTextures(texList, 2, textureSize, true);
            foreach(Texture2D frameTex in texList)
            {
                GameObject.Destroy(frameTex);
            }

            callback?.Invoke(texture, atlas, delays, width, height);
        }

        private static int GetTextureSize(AnimationInfo frameInfo, int i)
        {
            int testNum = 2;
            int numFramesInRow;
            int numFramesInColumn;
            while (true) {
                int numFrames = frameInfo.frameCount;
                // Make sure the number of frames is cleanly divisible by our testNum
                if (numFrames % testNum == 0)
                    numFrames += numFrames % testNum;

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

            int textureWidth = Mathf.Clamp(numFramesInRow * frameInfo.frames[i].width, 0, 4096);
            int textureHeight = Mathf.Clamp(numFramesInColumn * frameInfo.frames[i].height, 0, 4096);
            return Mathf.Max(textureWidth, textureHeight);
        }
    }
}
