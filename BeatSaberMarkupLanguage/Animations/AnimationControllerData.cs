﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationControllerData
    {
        public Sprite sprite;

        public int uvIndex = 0;
        public DateTime lastSwitch = DateTime.UtcNow;
        public Rect[] uvs;
        public float[] delays;
        public Sprite[] sprites;
        public bool IsPlaying { get; set; } = true;
        public Material animMaterial;

        public List<Image> activeImages = new List<Image>();
        
        public AnimationControllerData(Texture2D tex, Rect[] uvs, float[] delays)
        {
            sprites = new Sprite[uvs.Length];
            for(int i = 0; i < uvs.Length; i++)
                sprites[i] = Sprite.Create(tex, new Rect(uvs[i].x * tex.width, uvs[i].y * tex.height, uvs[i].width * tex.width, uvs[i].height * tex.height), new Vector2(0, 0), 100f);

            sprite = Utilities.LoadSpriteFromTexture(tex);
            this.uvs = uvs;
            this.delays = delays;
        }

        internal void CheckFrame(DateTime now)
        {
            if (activeImages.Count == 0)
                return;
            TimeSpan difference = now - lastSwitch;
            if (difference.Milliseconds < delays[uvIndex])
                return;

            lastSwitch = now;
            do
            {
                uvIndex++;
                if (uvIndex >= uvs.Length)
                    uvIndex = 0;
            }
            while (delays[uvIndex] == 0);

            foreach (Image image in activeImages)
            {
                image.sprite = sprites[uvIndex];
            }
        }
    }
}
