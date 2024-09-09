﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationController : ZenjectSingleton<AnimationController>, IInitializable, ITickable
    {
        private readonly Dictionary<string, AnimationControllerData> registeredAnimations = new();

        private AnimationController()
        {
            RegisteredAnimations = new ReadOnlyDictionary<string, AnimationControllerData>(registeredAnimations);
        }

        public ReadOnlyDictionary<string, AnimationControllerData> RegisteredAnimations { get; }

        public AnimationControllerData LoadingAnimation { get; private set; }

        public AnimationControllerData Register(string identifier, AnimationData animationData)
        {
            return Register(identifier, animationData.Atlas, animationData.Uvs, animationData.Delays);
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

        public void Initialize()
        {
            InitializeLoadingAnimation().ContinueWith((task) => Logger.Log.Error($"Failed to initialize loading animation\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
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

        private async Task InitializeLoadingAnimation()
        {
            AnimationData animationData = await AnimationLoader.ProcessApngAsync(await Utilities.GetResourceAsync(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Resources.loading.apng"));
            LoadingAnimation = new AnimationControllerData(animationData);
            registeredAnimations.Add("LOADING_ANIMATION", LoadingAnimation);
        }
    }
}
