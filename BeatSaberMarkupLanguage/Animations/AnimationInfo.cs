using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Animations
{
    public struct FrameInfo
    {
        public FrameInfo(int width, int height, int bytesPerPixel = 4)
        {
            this.Width = width;
            this.Height = height;
            Colors = new byte[width * height * bytesPerPixel];
            this.Delay = 0;
        }

        public int Width { get; }

        public int Height { get; }

        public byte[] Colors { get; }

        public int Delay { get; set; }
    }

    public record AnimationInfo(List<FrameInfo> Frames);
}
