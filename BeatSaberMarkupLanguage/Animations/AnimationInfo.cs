using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Animations
{
    public struct FrameInfo
    {
        public int Width;
        public int Height;
        public byte[] Colors;
        public int Delay;

        public FrameInfo(int width, int height, int bpp = 4)
        {
            this.Width = width;
            this.Height = height;
            Colors = new byte[width * height * bpp];
            this.Delay = 0;
        }
    }

    public record AnimationInfo
    {
        public List<FrameInfo> Frames;

        [Obsolete("Use frames.Count instead")]
        public int FrameCount = 0;

        [Obsolete]
        public bool Initialized = false;
    }
}
