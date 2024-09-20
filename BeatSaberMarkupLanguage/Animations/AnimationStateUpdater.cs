using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationStateUpdater : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        private AnimationControllerData controllerData;

        public Image Image
        {
            get => image;
            set => image = value;
        }

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
                ControllerData.ActiveImages.Add(image);
                image.sprite = ControllerData.Sprites[ControllerData.UvIndex];
            }
        }

        protected void OnDisable()
        {
            ControllerData?.ActiveImages.Remove(image);
        }

        protected void OnDestroy()
        {
            ControllerData?.ActiveImages.Remove(image);
        }
    }
}
