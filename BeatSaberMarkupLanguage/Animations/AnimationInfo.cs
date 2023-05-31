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

    public class AnimationInfo
    {
        public List<FrameInfo> frames;
        public int frameCount = 0;
        public bool initialized = false;
    }
}
