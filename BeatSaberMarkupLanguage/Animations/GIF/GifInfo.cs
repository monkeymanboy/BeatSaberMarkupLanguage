using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Animations.GIF
{
    public class GifInfo
    {
        public List<FrameInfo> frames = new List<FrameInfo>();
        public int frameCount = 0;
        public bool initialized = false;
        public bool isDelayConsistent = true;
    }
}
