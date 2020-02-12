using BeatSaberMarkupLanguage.Animations.GIF;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class APNGUnityDecoder
    {
        public static IEnumerator Process(APNG apng, Action<Texture2D, Rect[], float[], int, int> callback)
        {
            List<Texture2D> texList = new List<Texture2D>();
            DateTime startTime = DateTime.Now;

            int textureSize = 2048, width = 0, height = 0;
            Texture2D texture = null;
            List<float> delays = new List<float>();
            for (int i = 0; i < apng.FrameCount; i++)
            {
                if (texture == null)
                {
                    
                    textureSize = GetTextureSize(apng, i);
                    texture = new Texture2D(textureSize, textureSize);
                    
                }
                
                Frame curFrame = apng.Frames[i];
                delays.Add(curFrame.FrameRate);

                var frameTexture = new Texture2D((int)curFrame.fcTLChunk.Width, (int)curFrame.fcTLChunk.Height, TextureFormat.RGBA32, false);
                //frameTexture.LoadImage(curFrame.GetStream().ToArray());
                
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                try
                {
                    frameTexture.SetPixels32(CalculateColors(curFrame));
                    frameTexture.Apply(i == 0);
                }
                catch
                {
                    yield break;
                }
                yield return null;
                if (i == 0)
                {
                    width = (int)curFrame.fcTLChunk.Width;
                    height = (int)curFrame.fcTLChunk.Height;
                }
                texList.Add(frameTexture);
            }
            Rect[] atlas = texture.PackTextures(texList.ToArray(), 2, textureSize, true); //Okay, Tupper
            yield return null;
            callback?.Invoke(texture, atlas, delays.ToArray(), width, height);
        }

        private static Color32[] CalculateColors(Frame active)
        {
            Color32[] colors = null;

            System.Drawing.Bitmap bitmap = active.ToBitmap();
            System.Drawing.Graphics.FromImage(bitmap).DrawImage(System.Drawing.Image.FromStream(active.GetStream()), System.Drawing.Point.Empty);
            LockBitmap frame = new LockBitmap(bitmap);
            frame.LockBits();
            if (colors == null)
                colors = new Color32[frame.Height * frame.Width];
            for (int x = 0; x < frame.Width; x++)
            {
                for (int y = 0; y < frame.Height; y++)
                {
                    System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                    colors[(frame.Height - y - 1) * frame.Width + x] = new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A);
                }
            }
            return colors;
        }

        private static int GetTextureSize(APNG apng, int i)
        {
            int testNum = 2;
            retry:
            int numFrames = apng.FrameCount;
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

            var textureWidth = Mathf.Clamp(numFramesInRow * (int)apng.Frames[i].fcTLChunk.Width, 0, 2048);
            var textureHeight = Mathf.Clamp(numFramesInColumn * (int)apng.Frames[i].fcTLChunk.Height, 0, 2048);
            return Mathf.Max(textureWidth, textureHeight);
        }
    }
}
