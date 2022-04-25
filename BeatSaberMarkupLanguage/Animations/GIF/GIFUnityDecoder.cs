using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
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

		private static void ProcessingThread(byte[] gifData, AnimationInfo animationInfo) {
			System.Drawing.Image gifImage = System.Drawing.Image.FromStream(new MemoryStream(gifData));
			FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
			int frameCount = gifImage.GetFrameCount(dimension);

			animationInfo.frameCount = frameCount;
			animationInfo.initialized = true;
			animationInfo.frames = new List<FrameInfo>(frameCount);

			int firstDelayValue = -1;

			var delays = gifImage.GetPropertyItem(20736).Value;

			for(int i = 0; i < frameCount; i++) {
				gifImage.SelectActiveFrame(dimension, i);

				using(Bitmap bitmap = new Bitmap(gifImage)) {
					bitmap.MakeTransparent(System.Drawing.Color.Black);
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

					BitmapData frame = bitmap.LockBits(new Rectangle(Point.Empty, gifImage.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					FrameInfo currentFrame = new FrameInfo(frame.Width, frame.Height);

					Marshal.Copy(frame.Scan0, currentFrame.colors, 0, currentFrame.colors.Length);

					int delayPropertyValue = BitConverter.ToInt32(delays, i * 4);
					if(firstDelayValue == -1)
						firstDelayValue = delayPropertyValue;

					currentFrame.delay = delayPropertyValue * 10;
					animationInfo.frames.Add(currentFrame);
				}
			}
		}
    }
}
