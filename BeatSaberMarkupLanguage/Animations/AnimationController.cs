using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationController : PersistentSingleton<AnimationController>
    {
        private Dictionary<string, AnimationControllerData> registeredAnimations = new Dictionary<string, AnimationControllerData>();
        
        public AnimationControllerData Register(string identifier, Texture2D tex, Rect[] uvs, float[] delays)
        {
            if(!registeredAnimations.TryGetValue(identifier, out AnimationControllerData animationData))
            {
                animationData = new AnimationControllerData(tex, uvs, delays);
                registeredAnimations.Add(identifier, animationData);
            }
            else
            {
                GameObject.Destroy(tex);//if the identifier exists then this texture is a duplicate so might as well destroy it and free some memory (this can happen if you try to load a gif twice before the first one finishes processing)
            }
            return animationData;
        }
        public bool IsRegistered(string identifier) => registeredAnimations.ContainsKey(identifier);
        public AnimationControllerData GetAnimationControllerData(string identifier) => registeredAnimations[identifier];

        public void Update()
        {
            DateTime now = DateTime.UtcNow;
            foreach (AnimationControllerData animation in registeredAnimations.Values)
                if (animation.IsPlaying == true)
                    animation.CheckFrame(now);
        }

        public void UnregisterAll()
        {
            registeredAnimations.Clear();
        }
    }
}
