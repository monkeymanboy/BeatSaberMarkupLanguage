using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationControllerData
    {
        public Sprite Sprite;

        public int UvIndex = 0;
        public DateTime LastSwitch = DateTime.UtcNow;
        public Rect[] Uvs;
        public float[] Delays;
        public Sprite[] Sprites;
        public Material AnimMaterial;
        public List<Image> ActiveImages = new();

        private readonly bool isDelayConsistent = true;

        public AnimationControllerData(Texture2D tex, Rect[] uvs, float[] delays)
        {
            Sprites = new Sprite[uvs.Length];
            float firstDelay = -1;
            for (int i = 0; i < uvs.Length; i++)
            {
                Sprites[i] = Sprite.Create(tex, new Rect(uvs[i].x * tex.width, uvs[i].y * tex.height, uvs[i].width * tex.width, uvs[i].height * tex.height), new Vector2(0, 0), 100f);

                if (i == 0)
                {
                    firstDelay = delays[i];
                }

                if (delays[i] != firstDelay)
                {
                    isDelayConsistent = false;
                }
            }

            Sprite = Utilities.LoadSpriteFromTexture(tex);
            this.Uvs = uvs;
            this.Delays = delays;
        }

        internal AnimationControllerData(AnimationData animationData)
            : this(animationData.Atlas, animationData.Uvs, animationData.Delays)
        {
        }

        public bool IsPlaying { get; set; } = true;

        internal void CheckFrame(DateTime now)
        {
            if (ActiveImages.Count == 0)
            {
                return;
            }

            double differenceMs = (now - LastSwitch).TotalMilliseconds;
            if (differenceMs < Delays[UvIndex])
            {
                return;
            }

            if (isDelayConsistent && Delays[UvIndex] <= 10 && differenceMs < 100)
            {
                // Bump animations with consistently 10ms or lower frame timings to 100ms
                return;
            }

            LastSwitch = now;
            do
            {
                UvIndex++;
                if (UvIndex >= Uvs.Length)
                {
                    UvIndex = 0;
                }
            }
            while (!isDelayConsistent && Delays[UvIndex] == 0);

            foreach (Image image in ActiveImages)
            {
                image.sprite = Sprites[UvIndex];
            }
        }
    }
}
