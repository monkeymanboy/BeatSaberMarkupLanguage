using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Color = System.Drawing.Color;

namespace BeatSaberMarkupLanguage
{
    public static class Utilities
    {
        private static Sprite _editIcon = null;

        public static Sprite EditIcon
        {
            get
            {
                if (!_editIcon)
                    _editIcon = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>().First(x => x.sprite?.name == "EditIcon").sprite;

                return _editIcon;
            }
        }

        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static List<T> GetListOfType<T>(params object[] constructorArgs)
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));

            return objects;
        }

        //yoinked from https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType())
                return null; // type mismatch

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (PropertyInfo pinfo in pinfos)
            {
                if (pinfo.CanWrite && pinfo.Name != "name")
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                    }
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);
            foreach (FieldInfo finfo in finfos)
                finfo.SetValue(comp, finfo.GetValue(other));

            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

        //end of yoink

        public static string EscapeXml(string source)
        {
            return source.Replace("\"", "&quot;")
                .Replace("\"", "&quot;")
                .Replace("&", "&amp;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        public static class ImageResources
        {
            private static Material noGlowMat;
            private static Sprite _blankSprite = null;

            public static Material NoGlowMat
            {
                get
                {
                    if (noGlowMat == null)
                    {
                        noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First());
                        noGlowMat.name = "UINoGlowCustom";
                    }

                    return noGlowMat;
                }
            }

            public static Sprite BlankSprite
            {
                get
                {
                    if (!_blankSprite)
                        _blankSprite = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);

                    return _blankSprite;
                }
            }
        }

        public static Texture2D FindTextureInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                    return LoadTextureRaw(GetResource(asm, newPath));
            }
            catch (Exception ex)
            {
                Logger.log?.Error("Unable to find texture in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
            }
            return null;
        }

        public static Sprite FindSpriteInAssembly(string path)
        {
            try
            {
                AssemblyFromPath(path, out Assembly asm, out string newPath);
                if (asm.GetManifestResourceNames().Contains(newPath))
                    return LoadSpriteRaw(GetResource(asm, newPath));
            }
            catch (Exception ex)
            {
                Logger.log?.Error("Unable to find sprite in assembly! (You must prefix path with 'assembly name:' if the assembly and root namespace don't have the same name) Exception: " + ex);
            }
            return null;
        }

        public static void AssemblyFromPath(string inputPath, out Assembly assembly, out string path)
        {
            string[] parameters = inputPath.Split(':');
            switch (parameters.Length)
            {
                case 1:
                    path = parameters[0];
                    assembly = Assembly.Load(path.Substring(0, path.IndexOf('.')));
                    break;
                case 2:
                    path = parameters[1];
                    assembly = Assembly.Load(parameters[0]);
                    break;
                default:
                    throw new Exception($"Could not process resource path {inputPath}");
            }
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(file))
                    return Tex2D;
            }
            return null;
        }

        public static Sprite LoadSpriteRaw(byte[] image, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (SpriteTexture)
                return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
            return null;
        }

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        public class GifUtilities
        {
            public class GifInfo
            {
                public List<FrameInfo> frames = new List<FrameInfo>();
                public int frameCount = 0;
                public bool initialized = false;
                public bool isDelayConsistent = true;
            };

            public class FrameInfo
            {
                public int width, height;
                public Color32[] colors;
                public int delay = 0;
                public FrameInfo(int width, int height)
                {
                    this.width = width;
                    this.height = height;
                }
            };

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

            public static IEnumerator ProcessTex(byte[] gifData, Action<List<Sprite>, float[], bool, int, int> callback)
            {
                List<Sprite> texList = new List<Sprite>();
                GifInfo frameInfo = new GifInfo();
                DateTime startTime = DateTime.Now;
                Task.Run(() => ProcessingThread(gifData, frameInfo));
                yield return new WaitUntil(() => { return frameInfo.initialized; });


                int  width = 0, height = 0;
                List<float> delays = new List<float>();
                for (int i = 0; i < frameInfo.frameCount; i++)
                {
                    if (frameInfo.frames.Count <= i)
                    {
                        yield return new WaitUntil(() => { return frameInfo.frames.Count > i; });
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
                    catch (Exception e)
                    {
                        yield break;
                    }
                    yield return null;

                    texList.Add(Utilities.LoadSpriteFromTexture(frameTexture));
                }
                callback?.Invoke(texList, delays.ToArray(), frameInfo.isDelayConsistent, width, height);
            }

            private static void ProcessingThread(byte[] gifData, GifInfo frameInfo)
            {
                var gifImage = BytesToImage(gifData);
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

            public class LockBitmap
            {
                Bitmap source = null;
                IntPtr Iptr = IntPtr.Zero;
                BitmapData bitmapData = null;

                public byte[] Pixels { get; set; }
                public int Depth { get; private set; }
                public int Width { get; private set; }
                public int Height { get; private set; }

                public LockBitmap(Bitmap source)
                {
                    this.source = source;
                }

                /// <summary>
                /// Lock bitmap data
                /// </summary>
                public void LockBits()
                {
                    try
                    {
                        // Get width and height of bitmap
                        Width = source.Width;
                        Height = source.Height;

                        // get total locked pixels count
                        int PixelCount = Width * Height;

                        // Create rectangle to lock
                        Rectangle rect = new Rectangle(0, 0, Width, Height);

                        // get source bitmap pixel format size
                        Depth = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat);

                        // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                        if (Depth != 8 && Depth != 24 && Depth != 32)
                        {
                            throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                        }

                        // Lock bitmap and return bitmap data
                        bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                                     source.PixelFormat);

                        // create byte array to copy pixel values
                        int step = Depth / 8;
                        Pixels = new byte[PixelCount * step];
                        Iptr = bitmapData.Scan0;

                        // Copy data from pointer to array
                        Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                /// <summary>
                /// Unlock bitmap data
                /// </summary>
                public void UnlockBits()
                {
                    try
                    {
                        // Copy data from byte array to pointer
                        Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                        // Unlock bitmap data
                        source.UnlockBits(bitmapData);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                /// <summary>
                /// Get the color of the specified pixel
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public Color GetPixel(int x, int y)
                {
                    Color clr = Color.Empty;

                    // Get color components count
                    int cCount = Depth / 8;

                    // Get start index of the specified pixel
                    int i = ((y * Width) + x) * cCount;

                    if (i > Pixels.Length - cCount)
                        throw new IndexOutOfRangeException();

                    if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
                    {
                        byte b = Pixels[i];
                        byte g = Pixels[i + 1];
                        byte r = Pixels[i + 2];
                        byte a = Pixels[i + 3]; // a
                        clr = Color.FromArgb(a, r, g, b);
                    }
                    if (Depth == 24) // For 24 bpp get Red, Green and Blue
                    {
                        byte b = Pixels[i];
                        byte g = Pixels[i + 1];
                        byte r = Pixels[i + 2];
                        clr = Color.FromArgb(r, g, b);
                    }
                    if (Depth == 8)
                    // For 8 bpp get color value (Red, Green and Blue values are the same)
                    {
                        byte c = Pixels[i];
                        clr = Color.FromArgb(c, c, c);
                    }
                    return clr;
                }

                /// <summary>
                /// Set the color of the specified pixel
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="color"></param>
                public void SetPixel(int x, int y, Color color)
                {
                    // Get color components count
                    int cCount = Depth / 8;

                    // Get start index of the specified pixel
                    int i = ((y * Width) + x) * cCount;

                    if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                    {
                        Pixels[i] = color.B;
                        Pixels[i + 1] = color.G;
                        Pixels[i + 2] = color.R;
                        Pixels[i + 3] = color.A;
                    }
                    if (Depth == 24) // For 24 bpp set Red, Green and Blue
                    {
                        Pixels[i] = color.B;
                        Pixels[i + 1] = color.G;
                        Pixels[i + 2] = color.R;
                    }
                    if (Depth == 8)
                    // For 8 bpp set color value (Red, Green and Blue values are the same)
                    {
                        Pixels[i] = color.B;
                    }
                }
            }

            public static System.Drawing.Image BytesToImage(byte[] bytes) => System.Drawing.Image.FromStream(new MemoryStream(bytes));
        }
    }
}
