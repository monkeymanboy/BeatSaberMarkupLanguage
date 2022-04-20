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
		/*
        * Stock
[INFO @ 20:54:48 | _] john-xina-xina.gif X: 178px, Y: 225px, Size: 1978661 bytes, Load Time: 3,3187769
[INFO @ 20:54:48 | _] amongusdance2.gif X: 69px, Y: 69px, Size: 146616 bytes, Load Time: 3,5018037
[INFO @ 20:54:48 | _] henry.gif X: 198px, Y: 112px, Size: 3077066 bytes, Load Time: 3,5776917
[INFO @ 20:54:49 | _] spongebob_janitor.gif X: 150px, Y: 146px, Size: 368442 bytes, Load Time: 4,3347777
[INFO @ 20:54:53 | _] tim.gif X: 84px, Y: 84px, Size: 1833534 bytes, Load Time: 8,1682482
       */

		/*
		 * No yield null & bigger atlas
		 * [INFO @ 21:13:21 | _] amongusdance2.gif X: 69px, Y: 69px, Size: 146616 bytes, Load Time: 1,6368894
		 * [INFO @ 21:13:21 | _] spongebob_janitor.gif X: 200px, Y: 195px, Size: 368442 bytes, Load Time: 1,9340677
			[INFO @ 21:13:21 | _] john-xina-xina.gif X: 238px, Y: 300px, Size: 1978661 bytes, Load Time: 1,9340806
			[INFO @ 21:13:21 | _] henry.gif X: 352px, Y: 200px, Size: 3077066 bytes, Load Time: 1,9348954
			[INFO @ 21:13:21 | _] ifuk.gif X: 320px, Y: 228px, Size: 1869002 bytes, Load Time: 2,0869685
			[INFO @ 21:13:22 | _] tim.gif X: 202px, Y: 202px, Size: 1833534 bytes, Load Time: 3,2519967
		*/



		/*
         * the rest of the owl
[INFO @ 21:12:17 | _] amongusdance2.gif X: 69px, Y: 69px, Size: 146616 bytes, Load Time: 1,2073083
[INFO @ 21:12:18 | _] spongebob_janitor.gif X: 200px, Y: 195px, Size: 368442 bytes, Load Time: 1,4864104
[INFO @ 21:12:18 | _] john-xina-xina.gif X: 238px, Y: 300px, Size: 1978661 bytes, Load Time: 1,4865139
[INFO @ 21:12:18 | _] henry.gif X: 352px, Y: 200px, Size: 3077066 bytes, Load Time: 1,4875473
[INFO @ 21:12:18 | _] tim.gif X: 202px, Y: 202px, Size: 1833534 bytes, Load Time: 2,2332462
        */

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
					bitmap.MakeTransparent();
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

					BitmapData frame = bitmap.LockBits(new Rectangle(Point.Empty, gifImage.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					FrameInfo currentFrame = new FrameInfo(frame.Width, frame.Height);

					Marshal.Copy(frame.Scan0, currentFrame.colors, 0, currentFrame.colors.Length);

					bitmap.UnlockBits(frame);

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
