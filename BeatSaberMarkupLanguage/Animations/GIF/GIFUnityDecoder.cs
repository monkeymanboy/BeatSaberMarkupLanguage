using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class GIFUnityDecoder
    {
        public static IEnumerator Process(byte[] gifData, Action<AnimationInfo> callback)
        {
            AnimationInfo animationInfo = new AnimationInfo();
            Task.Run(() => ProcessingThread(gifData, animationInfo));
            yield return new WaitUntil(() => { return animationInfo.initialized; });
            callback?.Invoke(animationInfo);
        }
        
        private static void ProcessingThread(byte[] gifData, AnimationInfo animationInfo)
        {
            System.Drawing.Image gifImage = System.Drawing.Image.FromStream(new MemoryStream(gifData));
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);

            animationInfo.frameCount = frameCount;
            animationInfo.initialized = true;

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

                currentFrame.delay = delayPropertyValue * 10;
                animationInfo.frames.Add(currentFrame);
                index += 4;

                Thread.Sleep(0);
            }
        }
    }
}
