using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationStateUpdater : MonoBehaviour
    {
        public Image Image;

        private AnimationControllerData controllerData;

        public AnimationControllerData ControllerData
        {
            get => controllerData;
            set
            {
                if (controllerData != null)
                {
                    OnDisable();
                }

                controllerData = value;

                if (isActiveAndEnabled)
                {
                    OnEnable();
                }
            }
        }

        protected void OnEnable()
        {
            if (ControllerData != null)
            {
                ControllerData.ActiveImages.Add(Image);
                Image.sprite = ControllerData.Sprites[ControllerData.UvIndex];
            }
        }

        protected void OnDisable()
        {
            ControllerData?.ActiveImages.Remove(Image);
        }

        protected void OnDestroy()
        {
            ControllerData?.ActiveImages.Remove(Image);
        }
    }
}
