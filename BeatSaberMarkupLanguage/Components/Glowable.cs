using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Glowable : MonoBehaviour
    {
        public Image Image;

        public void SetGlow(string colorString)
        {
            if (Image == null)
            {
                return;
            }

            if (colorString != "none")
            {
                Image.color = Parse.Color(colorString);
                Image.gameObject.SetActive(true);
            }
            else
            {
                Image.gameObject.SetActive(false);
            }
        }
    }
}
