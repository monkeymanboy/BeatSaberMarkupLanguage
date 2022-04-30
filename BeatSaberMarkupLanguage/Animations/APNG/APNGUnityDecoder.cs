using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class APNGUnityDecoder
    {
        public static IEnumerator Process(byte[] apngData, Action<AnimationInfo> callback)
        {
            AnimationInfo animationInfo = new AnimationInfo();
            Task.Run(() => ProcessingThread(apngData, animationInfo));
            yield return new WaitUntil(() => { return animationInfo.initialized; });
            callback?.Invoke(animationInfo);
        }

		const float byteInverse = 1f / 255f;

		private static void ProcessingThread(byte[] apngData, AnimationInfo animationInfo)
        {
			APNG.APNG apng = APNG.APNG.FromStream(new System.IO.MemoryStream(apngData));
			int frameCount = apng.FrameCount;
			animationInfo.frameCount = frameCount;
			animationInfo.initialized = true;

			animationInfo.frames = new System.Collections.Generic.List<FrameInfo>(frameCount);

			FrameInfo prevFrame = default;

			for(int i = 0; i < frameCount; i++) {
				Frame apngFrame = apng.Frames[i];

				using(Bitmap bitmap = apngFrame.ToBitmap()) {
					FrameInfo frameInfo = new FrameInfo(bitmap.Width, bitmap.Height);

					bitmap.MakeTransparent(System.Drawing.Color.Black);
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

					BitmapData frame = bitmap.LockBits(new Rectangle(Point.Empty, apng.ActualSize), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

					Marshal.Copy(frame.Scan0, frameInfo.colors, 0, frameInfo.colors.Length);

					bitmap.UnlockBits(frame);

					if(apngFrame.fcTLChunk.BlendOp == APNG.Chunks.BlendOps.APNGBlendOpOver && i > 0) {
						// BGRA
						var last = prevFrame.colors;
						var src = frameInfo.colors;

						for(var clri = frameInfo.colors.Length - 1; i > 2; i -= 4) {
							var srcA = src[clri - 3] * byteInverse;
							var lastA = last[clri - 3] * byteInverse;

							float blendedA = srcA + (1 - srcA) * lastA;
							src[clri - 3] = (byte)Math.Round(blendedA * 255);

							for(var c = 0; c < 3; c++) {
								var srcC = src[clri - i] * byteInverse;
								var lastC = last[clri - i] * byteInverse;

								src[clri - i] = (byte)Math.Round((srcA * srcC + (1 - srcA) * lastA * lastC * 255f) / blendedA);
							}
						}
					}

					frameInfo.delay = apngFrame.FrameRate;
					animationInfo.frames.Add(frameInfo);
					prevFrame = frameInfo;
				}
			}
		}
    }
}
