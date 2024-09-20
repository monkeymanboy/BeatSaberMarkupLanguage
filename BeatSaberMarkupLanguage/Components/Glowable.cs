using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Glowable : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        public Image Image
        {
            get => image;
            set => image = value;
        }

        public void SetGlow(string colorString)
        {
            if (image == null)
            {
                return;
            }

            if (colorString != "none")
            {
                image.color = Parse.Color(colorString);
                image.gameObject.SetActive(true);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}
