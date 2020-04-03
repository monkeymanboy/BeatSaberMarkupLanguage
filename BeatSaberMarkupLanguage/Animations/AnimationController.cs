using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationController : PersistentSingleton<AnimationController>
    {
        private Dictionary<string, AnimationControllerData> registeredAnimations = new Dictionary<string, AnimationControllerData>();
        public IReadOnlyDictionary<string, AnimationControllerData> RegisteredAnimations;
        public AnimationControllerData loadingAnimation;

        private void Awake()
        {
            RegisteredAnimations = new ReadOnlyDictionary<string, AnimationControllerData>(registeredAnimations);
        }
        
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

        public void InitializeLoadingAnimation()
        {
            AnimationLoader.Process(AnimationType.APNG, Utilities.GetResource(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Resources.loading.apng"), (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
            {
                loadingAnimation = new AnimationControllerData(tex, uvs, delays);
                registeredAnimations.Add("LOADING_ANIMATION", loadingAnimation);
            });
        }

        public void Update()
        {
            DateTime now = DateTime.UtcNow;
            foreach (AnimationControllerData animation in registeredAnimations.Values)
                if (animation.IsPlaying == true)
                    animation.CheckFrame(now);
        }
    }
}
