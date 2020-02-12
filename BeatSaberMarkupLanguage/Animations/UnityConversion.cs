using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class UnityConversion
    {
        public Texture2D[] GetAPNGFramesAsTextures(byte[] bytes) => GetAPNGFramesAsTextures(new MemoryStream(bytes));

        public Texture2D[] GetAPNGFramesAsTextures(MemoryStream stream)
        {
            APNG apng = new APNG();
            apng.Load(stream);
            Frame[] frames = apng.Frames;
            Texture2D[] texs = new Texture2D[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(frames[i].GetStream().ToArray());
                texs[i] = tex;
            }
            return texs;
        }
    }
}
