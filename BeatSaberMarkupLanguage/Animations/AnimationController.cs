using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationController : PersistentSingleton<AnimationController>, ITickable
    {
        public ReadOnlyDictionary<string, AnimationControllerData> RegisteredAnimations;
        public AnimationControllerData loadingAnimation;

        private readonly Dictionary<string, AnimationControllerData> registeredAnimations = new();

        public AnimationController()
        {
            RegisteredAnimations = new ReadOnlyDictionary<string, AnimationControllerData>(registeredAnimations);
        }

        public AnimationControllerData Register(string identifier, AnimationData animationData)
        {
            return Register(identifier, animationData.atlas, animationData.uvs, animationData.delays);
        }

        public AnimationControllerData Register(string identifier, Texture2D tex, Rect[] uvs, float[] delays)
        {
            if (!registeredAnimations.TryGetValue(identifier, out AnimationControllerData animationData))
            {
                animationData = new AnimationControllerData(tex, uvs, delays);
                registeredAnimations.Add(identifier, animationData);
            }
            else
            {
                UnityEngine.Object.Destroy(tex); // if the identifier exists then this texture is a duplicate so might as well destroy it and free some memory (this can happen if you try to load a gif twice before the first one finishes processing)
            }

            return animationData;
        }

        public void Tick()
        {
            DateTime now = DateTime.UtcNow;
            foreach (AnimationControllerData animation in registeredAnimations.Values)
            {
                if (animation.IsPlaying)
                {
                    animation.CheckFrame(now);
                }
            }
        }

        internal async Task InitializeLoadingAnimation()
        {
            AnimationData animationData = await AnimationLoader.ProcessApngAsync(await Utilities.GetResourceAsync(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Resources.loading.apng"));
            loadingAnimation = new AnimationControllerData(animationData);
            registeredAnimations.Add("LOADING_ANIMATION", loadingAnimation);
        }
    }
}
