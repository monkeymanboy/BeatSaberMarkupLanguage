using System;
using System.Collections;
using UnityEngine;

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
            int textureSize = 2048, width = 0, height = 0;
            Texture2D texture = null;
            Texture2D[] texList = new Texture2D[animationInfo.frameCount];
            float[] delays = new float[animationInfo.frameCount];
            for (int i = 0; i < animationInfo.frameCount; i++)
            {
                if (animationInfo.frames.Count <= i)
                {
                    yield return new WaitUntil(() => { return animationInfo.frames.Count > i; });
                }

                if (texture == null)
                {
                    textureSize = GetTextureSize(animationInfo, i);
                    texture = new Texture2D(animationInfo.frames[i].width, animationInfo.frames[i].height);
                }
                
                FrameInfo currentFrameInfo = animationInfo.frames[i];
                delays[i] = currentFrameInfo.delay;

                Texture2D frameTexture = new Texture2D(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.RGBA32, false);
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                try
                {
                    frameTexture.SetPixels32(currentFrameInfo.colors);
                    frameTexture.Apply(i == 0);
                }
                catch
                {
                    yield break;
                }
                yield return null;

                texList[i] = frameTexture;

                if (i == 0)
                {
                    width = animationInfo.frames[i].width;
                    height = animationInfo.frames[i].height;
                }
            }
            Rect[] atlas = texture.PackTextures(texList, 2, textureSize, true);
            foreach(Texture2D frameTex in texList)
            {
                GameObject.Destroy(frameTex);
            }
            yield return null;

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
                if (!(numFrames % testNum != 0))
                    numFrames += numFrames % testNum;

                numFramesInRow = numFrames / testNum;
                numFramesInColumn = numFrames / numFramesInRow;
                
                if (numFramesInRow <= numFramesInColumn)
                {
                    break;
                }
                testNum += 2;
            }

            int textureWidth = Mathf.Clamp(numFramesInRow * frameInfo.frames[i].width, 0, 2048);
            int textureHeight = Mathf.Clamp(numFramesInColumn * frameInfo.frames[i].height, 0, 2048);
            return Mathf.Max(textureWidth, textureHeight);
        }
    }
}
