using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    [RequireComponent(typeof(Image))]
    public class GIFAnimationController : MonoBehaviour
    {
        private Image img;
        public Sprite[] frames;
        public DateTime lastSwitch = DateTime.UtcNow;
        public int fps = 12;
        public int index = 0;
        public float[] delays;
        private bool status = true;
        public bool isDelayConsistent = true;

        public void Toggle() => status = !status;
        public void Reset() => index = 0;

        void Awake()
        {
            img = GetComponent<Image>();
            //Prevent overriding the original material
            img.material = new Material(img?.material);
        }


        void Update()
        {
            if (status)
            {
                var now = DateTime.UtcNow;
                if (img == null || frames.Length == 0)
                    return;

                var difference = now - lastSwitch;
                if (difference.Milliseconds < delays[index])
                    return;

                lastSwitch = now;

                if (status && frames.Length > 0)
                {
                    img.sprite = frames[index];
                    index++;
                    if (index >= frames.Length)
                        index = 0;
                }
            }
        }
    }
}
