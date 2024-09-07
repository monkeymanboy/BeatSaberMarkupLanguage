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
        // https://learn.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.id?view=netframework-4.7.2
        private const int PropertyTagFrameDelay = 0x5100;

        // frame delay is in 100ths of a second; we want milliseconds
        // https://www.w3.org/Graphics/GIF/spec-gif89a.txt (section 23, point vii)
        private const int FrameDelayToMillisecondsRatio = 10;

        [Obsolete("Use ProcessAsync instead.")]
        public static IEnumerator Process(byte[] gifData, Action<AnimationInfo> callback)
        {
            AnimationInfo animationInfo = new();
            Task.Run(() => ProcessingThread(gifData, animationInfo));
            yield return new WaitUntil(() => { return animationInfo.Initialized; });
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
            animationInfo.FrameCount = frameCount;
            animationInfo.Initialized = true;
#pragma warning restore CS0612, CS0618
            animationInfo.Frames = new List<FrameInfo>(frameCount);

            // TODO: detect static GIFs earlier so we don't create all the animation stuff for no reason
            // FF FF FF 7F is int.MaxValue (little endian)
            byte[] delays = frameCount > 1 ? gifImage.GetPropertyItem(PropertyTagFrameDelay).Value : new byte[] { 0xFF, 0xFF, 0xFF, 0x7F };
            Rectangle rect = new(Point.Empty, gifImage.Size);

            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);

                using (Bitmap bitmap = new(gifImage))
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                    BitmapData frame = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    FrameInfo currentFrame = new(frame.Width, frame.Height);

                    Marshal.Copy(frame.Scan0, currentFrame.Colors, 0, currentFrame.Colors.Length);

                    int delayPropertyValue = BitConverter.ToInt32(delays, i * 4);
                    currentFrame.Delay = delayPropertyValue * FrameDelayToMillisecondsRatio;
                    animationInfo.Frames.Add(currentFrame);
                }
            }
        }
    }
}
