using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Animations
{
    public struct FrameInfo
    {
        public int width;
        public int height;
        public byte[] colors;
        public int delay;

        public FrameInfo(int width, int height, int bpp = 4)
        {
            this.width = width;
            this.height = height;
            colors = new byte[width * height * bpp];
            this.delay = 0;
        }
    }

    public record AnimationInfo
    {
        public List<FrameInfo> frames;

        [Obsolete("Use frames.Count instead")]
        public int frameCount = 0;

        [Obsolete]
        public bool initialized = false;
    }
}
