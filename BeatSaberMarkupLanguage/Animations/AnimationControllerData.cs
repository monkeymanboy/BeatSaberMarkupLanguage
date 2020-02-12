using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationControllerData
    {
        public Sprite sprite;
        public int uvIndex = 0;
        public DateTime lastSwitch = DateTime.UtcNow;
        public Rect[] uvs;
        public float[] delays;
        public int RefCtr { get; private set; } = 0;
        public bool IsPlaying { get; set; } = true;
        public Material animMaterial;

        public void IncRefs()
        {
            lock (this)
            {
                if (RefCtr == 0)
                {
                    uvIndex = 0;
                    lastSwitch = DateTime.UtcNow;
                }
                RefCtr++;
            }
        }
        public void DecRefs()
        {
            lock (this)
            {
                RefCtr--;
                if (RefCtr < 0)
                    RefCtr = 0;
            }
        }

        public AnimationControllerData(Texture2D tex, Rect[] uvs, float[] delays)
        {
            if (animMaterial == null)
            {
                animMaterial = UnityEngine.Object.Instantiate(Utilities.GifUtilities.CropMaterial);
                animMaterial.SetVector("_CropFactors", new Vector4(uvs[0].x, uvs[0].y, uvs[0].width, uvs[0].height));
            }
            animMaterial.mainTexture = tex;
            sprite = Utilities.LoadSpriteFromTexture(tex);
            this.uvs = uvs;
            this.delays = delays;
        }
    }
}
