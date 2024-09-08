﻿using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class APNGUnityDecoder
    {
        private const float ByteInverse = 1f / 255f;

        [Obsolete("Use ProcessAsync instead.")]
        public static IEnumerator Process(byte[] apngData, Action<AnimationInfo> callback)
        {
            AnimationInfo animationInfo = new();
            Task.Run(() => ProcessingThread(apngData, animationInfo));
            yield return new WaitUntil(() => { return animationInfo.Initialized; });
            callback?.Invoke(animationInfo);
        }

        public static Task<AnimationInfo> ProcessAsync(byte[] apngData)
        {
            return Task.Run(() =>
            {
                AnimationInfo animationInfo = new();
                ProcessingThread(apngData, animationInfo);
                return animationInfo;
            });
        }

        private static void ProcessingThread(byte[] apngData, AnimationInfo animationInfo)
        {
            APNG.APNG apng = APNG.APNG.FromStream(new System.IO.MemoryStream(apngData));
            int frameCount = apng.FrameCount;
#pragma warning disable CS0612, CS0618
            animationInfo.FrameCount = frameCount;
            animationInfo.Initialized = true;
#pragma warning restore CS0612, CS0618

            animationInfo.Frames = new System.Collections.Generic.List<FrameInfo>(frameCount);

            FrameInfo prevFrame = default;

            for (int i = 0; i < frameCount; i++)
            {
                Frame apngFrame = apng.Frames[i];

                using (Bitmap bitmap = apngFrame.ToBitmap())
                {
                    FrameInfo frameInfo = new(bitmap.Width, bitmap.Height);

                    bitmap.MakeTransparent(System.Drawing.Color.Black);
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                    BitmapData frame = bitmap.LockBits(new Rectangle(Point.Empty, apng.ActualSize), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    Marshal.Copy(frame.Scan0, frameInfo.Colors, 0, frameInfo.Colors.Length);

                    bitmap.UnlockBits(frame);

                    if (apngFrame.FcTLChunk.BlendOp == APNG.Chunks.BlendOps.APNGBlendOpOver && i > 0)
                    {
                        // BGRA
                        byte[] last = prevFrame.Colors;
                        byte[] src = frameInfo.Colors;

                        for (int clri = frameInfo.Colors.Length - 1; i > 2; i -= 4)
                        {
                            float srcA = src[clri - 3] * ByteInverse;
                            float lastA = last[clri - 3] * ByteInverse;

                            float blendedA = srcA + ((1 - srcA) * lastA);
                            src[clri - 3] = (byte)Math.Round(blendedA * 255);

                            for (int c = 0; c < 3; c++)
                            {
                                float srcC = src[clri - i] * ByteInverse;
                                float lastC = last[clri - i] * ByteInverse;

                                src[clri - i] = (byte)Math.Round(((srcA * srcC) + ((1 - srcA) * lastA * lastC * 255f)) / blendedA);
                            }
                        }
                    }

                    frameInfo.Delay = apngFrame.FrameRate;
                    animationInfo.Frames.Add(frameInfo);
                    prevFrame = frameInfo;
                }
            }
        }
    }
}
