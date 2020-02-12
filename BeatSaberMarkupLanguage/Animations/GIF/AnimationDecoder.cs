using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations.GIF
{
    public class AnimationDecoder
    {
        public static IEnumerator Process(byte[] gifData, Action<Texture2D, Rect[], float[], bool, int, int> callback)
        {
            List<Texture2D> texList = new List<Texture2D>();
            GifInfo frameInfo = new GifInfo();
            DateTime startTime = DateTime.Now;
            Task.Run(() => ProcessingThread(gifData, frameInfo));
            yield return new WaitUntil(() => { return frameInfo.initialized; });

            int textureSize = 2048, width = 0, height = 0;
            Texture2D texture = null;
            List<float> delays = new List<float>();
            for (int i = 0; i < frameInfo.frameCount; i++)
            {
                if (frameInfo.frames.Count <= i)
                {
                    yield return new WaitUntil(() => { return frameInfo.frames.Count > i; });
                }

                if (texture == null)
                {
                    textureSize = GetTextureSize(frameInfo, i);
                    texture = new Texture2D(frameInfo.frames[i].width, frameInfo.frames[i].height);
                }

                FrameInfo currentFrameInfo = frameInfo.frames[i];
                delays.Add(currentFrameInfo.delay);

                var frameTexture = new Texture2D(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.RGBA32, false);
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

                texList.Add(frameTexture);

                if (i == 0)
                {
                    width = frameInfo.frames[i].width;
                    height = frameInfo.frames[i].height;
                }
            }
            Rect[] atlas = texture.PackTextures(texList.ToArray(), 2, textureSize, true);

            yield return null;

            callback?.Invoke(texture, atlas, delays.ToArray(), frameInfo.isDelayConsistent, width, height);
        }


        private static void ProcessingThread(byte[] gifData, GifInfo frameInfo)
        {
            var gifImage = System.Drawing.Image.FromStream(new MemoryStream(gifData));
            var dimension = new System.Drawing.Imaging.FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);

            frameInfo.frameCount = frameCount;
            frameInfo.initialized = true;

            int index = 0;
            int firstDelayValue = -1;
            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(gifImage.Width, gifImage.Height);
                System.Drawing.Graphics.FromImage(bitmap).DrawImage(gifImage, System.Drawing.Point.Empty);
                LockBitmap frame = new LockBitmap(bitmap);
                frame.LockBits();
                FrameInfo currentFrame = new FrameInfo(bitmap.Width, bitmap.Height);

                if (currentFrame.colors == null)
                    currentFrame.colors = new Color32[frame.Height * frame.Width];
                for (int x = 0; x < frame.Width; x++)
                {
                    for (int y = 0; y < frame.Height; y++)
                    {
                        System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                        currentFrame.colors[(frame.Height - y - 1) * frame.Width + x] = new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A);
                    }
                }

                int delayPropertyValue = BitConverter.ToInt32(gifImage.GetPropertyItem(20736).Value, index);
                if (firstDelayValue == -1)
                    firstDelayValue = delayPropertyValue;

                if (delayPropertyValue != firstDelayValue)
                {
                    frameInfo.isDelayConsistent = false;
                }

                currentFrame.delay = delayPropertyValue * 10;
                frameInfo.frames.Add(currentFrame);
                index += 4;
            }
        }

        private static int GetTextureSize(GifInfo frameInfo, int i)
        {
            int testNum = 2;
            retry:
            int numFrames = frameInfo.frameCount;
            // Make sure the number of frames is cleanly divisible by our testNum
            if (!(numFrames % testNum != 0))
                numFrames += numFrames % testNum;

            int numFramesInRow = numFrames / testNum;
            int numFramesInColumn = numFrames / numFramesInRow;

            if (numFramesInRow > numFramesInColumn)
            {
                testNum += 2;
                goto retry;
            }

            var textureWidth = Mathf.Clamp(numFramesInRow * frameInfo.frames[i].width, 0, 2048);
            var textureHeight = Mathf.Clamp(numFramesInColumn * frameInfo.frames[i].height, 0, 2048);
            return Mathf.Max(textureWidth, textureHeight);
        }
    }
}
