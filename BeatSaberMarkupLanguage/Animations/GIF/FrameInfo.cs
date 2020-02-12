using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations.GIF
{
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
    }
}
