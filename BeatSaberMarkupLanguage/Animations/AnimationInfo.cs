using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationInfo
    {
        public List<FrameInfo> frames;
        public int frameCount = 0;
        public bool initialized = false;
    }
    public struct FrameInfo
    {
        public int width, height;
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
}
