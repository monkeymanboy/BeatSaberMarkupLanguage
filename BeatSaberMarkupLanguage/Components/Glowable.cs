using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Glowable : MonoBehaviour
    {
        public Image image;

        public void SetGlow(string colorString)
        {
            if (image == null)
                return;
            if (colorString != "none")
            {
                if (!ColorUtility.TryParseHtmlString(colorString, out Color color))
                    Logger.log.Warn($"Invalid color: {colorString}");
                image.color = color;
                image.gameObject.SetActive(true);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}
