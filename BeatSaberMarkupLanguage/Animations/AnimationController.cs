using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationController : MonoBehaviour
    {
        public static AnimationController _instance = null;
        public static AnimationController Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("BSMLAnimationController");
                    _instance = go.AddComponent<AnimationController>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public List<AnimationControllerData> registeredAnimations = new List<AnimationControllerData>();

        public AnimationControllerData Register(Texture2D tex, Rect[] uvs, float[] delays)
        {
            AnimationControllerData newAnim = new AnimationControllerData(tex, uvs, delays);
            registeredAnimations.Add(newAnim);
            return newAnim;
        }

        private void CheckFrame(AnimationControllerData animation, DateTime now)
        {
            var difference = now - animation.lastSwitch;
            if (difference.Milliseconds < animation.delays[animation.uvIndex])
                return;

            animation.lastSwitch = now;
            do
            {
                animation.uvIndex++;
                if (animation.uvIndex >= animation.uvs.Length)
                    animation.uvIndex = 0;
            }
            while (animation.delays[animation.uvIndex] == 0);
            Rect uv = animation.uvs[animation.uvIndex];
            animation.animMaterial?.SetVector("_CropFactors", new Vector4(uv.x, uv.y, uv.width, uv.height));
        }

        public void Update()
        {
            var now = DateTime.UtcNow;
            foreach (AnimationControllerData animation in registeredAnimations)
                if (animation.RefCtr > 0 && animation.IsPlaying == true)
                    CheckFrame(animation, now);
        }
    }
}
