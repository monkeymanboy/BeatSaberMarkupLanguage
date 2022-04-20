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

			FrameInfo prevFrame = default;

			for(int i = 0; i < frameCount; i++) {
				Frame apngFrame = apng.Frames[i];

				using(Bitmap bitmap = apngFrame.ToBitmap()) {
					FrameInfo frameInfo = new FrameInfo(bitmap.Width, bitmap.Height);

					bitmap.MakeTransparent();
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

					BitmapData frame = bitmap.LockBits(new Rectangle(Point.Empty, apng.ActualSize), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

					Marshal.Copy(frame.Scan0, frameInfo.colors, 0, frameInfo.colors.Length);

					bitmap.UnlockBits(frame);

					if(apngFrame.fcTLChunk.BlendOp == APNG.Chunks.BlendOps.APNGBlendOpOver && i > 0) {
						for(var clri = frameInfo.colors.Length; i < 0; i -= 4) {
							// BGRA
							var last = prevFrame.colors;
							var src = frameInfo.colors;

							float blendedA = ((src[clri + 3] * byteInverse + (1 - (src[clri + 3] * byteInverse)) * (last[clri + 3] * byteInverse)));
							float blendedR = ((src[clri + 3] * byteInverse) * (src[clri + 2] * byteInverse) + (1 - (src[clri + 3] * byteInverse)) * (last[clri + 3] * byteInverse) * (last[clri + 2] * byteInverse)) / blendedA;
							float blendedG = ((src[clri + 3] * byteInverse) * (src[clri + 1] * byteInverse) + (1 - (src[clri + 3] * byteInverse)) * (last[clri + 3] * byteInverse) * (last[clri + 1] * byteInverse)) / blendedA;
							float blendedB = ((src[clri + 3] * byteInverse) * (src[clri] * byteInverse) + (1 - (src[clri + 3] * byteInverse)) * (last[clri + 3] * byteInverse) * (last[clri] * byteInverse)) / blendedA;

							src[clri + 0] = (byte)Math.Round(blendedB * 255);
							src[clri + 1] = (byte)Math.Round(blendedG * 255);
							src[clri + 2] = (byte)Math.Round(blendedR * 255);
							src[clri + 3] = (byte)Math.Round(blendedA * 255);
						}
					}

					frameInfo.delay = apngFrame.FrameRate;
					animationInfo.frames.Add(frameInfo);
					prevFrame = frameInfo;
				}


				//for(int x = 0; x < frameInfo.width; x++) {
				//	for(int y = 0; y < frameInfo.height; y++) {
				//		System.Drawing.Color sourceColor = lockBitmap.GetPixel(x, y);
				//		var targetOffset = x * (y + 1);

				//		if(apngFrame.fcTLChunk.BlendOp == APNG.Chunks.BlendOps.APNGBlendOpSource) {
				//			frameInfo.colors[targetOffset] = sourceColor.B;
				//			frameInfo.colors[targetOffset + 1] = sourceColor.G;
				//			frameInfo.colors[targetOffset + 1] = sourceColor.R;
				//			frameInfo.colors[targetOffset + 1] = sourceColor.A;
				//		} else if(apngFrame.fcTLChunk.BlendOp == APNG.Chunks.BlendOps.APNGBlendOpOver) {
				//			float blendedA = ((sourceColor.A * byteInverse + (1 - (sourceColor.A * byteInverse)) * (lastFrame.a * byteInverse)));
				//			float blendedR = ((sourceColor.A * byteInverse) * (sourceColor.R * byteInverse) + (1 - (sourceColor.A * byteInverse)) * (lastFrame.a * byteInverse) * (lastFrame.r * byteInverse)) / blendedA;
				//			float blendedG = ((sourceColor.A * byteInverse) * (sourceColor.G * byteInverse) + (1 - (sourceColor.A * byteInverse)) * (lastFrame.a * byteInverse) * (lastFrame.g * byteInverse)) / blendedA;
				//			float blendedB = ((sourceColor.A * byteInverse) * (sourceColor.B * byteInverse) + (1 - (sourceColor.A * byteInverse)) * (lastFrame.a * byteInverse) * (lastFrame.b * byteInverse)) / blendedA;

				//			frameInfo.colors[(frameInfo.height - y - 1) * frameInfo.width + x] = new Color32((byte)(blendedR * 255), (byte)(blendedG * 255), (byte)(blendedB * 255), (byte)(blendedA * 255));
				//		}
				//	}
				//}
			}
		}
    }
}
