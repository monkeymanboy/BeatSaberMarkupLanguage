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

        private void OnEnable()
        {
            if (ControllerData != null)
            {
                ControllerData.ActiveImages.Add(Image);
                Image.sprite = ControllerData.Sprites[ControllerData.UvIndex];
            }
        }

        private void OnDisable()
        {
            ControllerData?.ActiveImages.Remove(Image);
        }

        private void OnDestroy()
        {
            ControllerData?.ActiveImages.Remove(Image);
        }
    }
}
