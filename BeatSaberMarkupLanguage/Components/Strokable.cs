using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Strokable : MonoBehaviour
    {
        public Image image;

        public enum StrokeType
        {
            None,
            Clean,
            Regular,
        }

        public void SetType(StrokeType strokeType)
        {
            if (image == null)
            {
                return;
            }

            switch (strokeType)
            {
                case StrokeType.None:
                    image.enabled = false;
                    break;
                case StrokeType.Clean:
                    image.enabled = true;
                    image.sprite = Utilities.FindSpriteCached("RoundRectSmallStroke");
                    break;
                case StrokeType.Regular:
                    image.enabled = true;
                    image.sprite = Utilities.FindSpriteCached("RoundRectBigStroke");
                    break;
            }
        }

        public void SetColor(string strokeColor)
        {
            if (image == null)
            {
                return;
            }

            if (!ColorUtility.TryParseHtmlString(strokeColor, out Color color))
            {
                Logger.Log.Warn($"Invalid color: {strokeColor}");
            }

            image.color = color;
            image.enabled = true;
        }
    }
}
