using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class GIFUnityDecoder
    {
        [Obsolete("Use ProcessAsync instead.")]
        public static IEnumerator Process(byte[] gifData, Action<AnimationInfo> callback)
        {
            AnimationInfo animationInfo = new();
            Task.Run(() => ProcessingThread(gifData, animationInfo));
            yield return new WaitUntil(() => { return animationInfo.initialized; });
            callback?.Invoke(animationInfo);
        }

        public static Task<AnimationInfo> ProcessAsync(byte[] gifData)
        {
            return Task.Run(() =>
            {
                AnimationInfo animationInfo = new();
                ProcessingThread(gifData, animationInfo);
                return animationInfo;
            });
        }

        private static void ProcessingThread(byte[] gifData, AnimationInfo animationInfo)
        {
            Image gifImage = Image.FromStream(new MemoryStream(gifData));
            FrameDimension dimension = new(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);

#pragma warning disable CS0612, CS0618
            animationInfo.frameCount = frameCount;
            animationInfo.initialized = true;
#pragma warning restore CS0612, CS0618
            animationInfo.frames = new List<FrameInfo>(frameCount);

            byte[] delays = gifImage.GetPropertyItem(20736).Value;
            Rectangle rect = new(Point.Empty, gifImage.Size);

            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);

                using (Bitmap bitmap = new(gifImage))
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                    BitmapData frame = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    FrameInfo currentFrame = new(frame.Width, frame.Height);

                    Marshal.Copy(frame.Scan0, currentFrame.colors, 0, currentFrame.colors.Length);

                    int delayPropertyValue = BitConverter.ToInt32(delays, i * 4);
                    currentFrame.delay = delayPropertyValue * 10;
                    animationInfo.frames.Add(currentFrame);
                }
            }
        }
    }
}
