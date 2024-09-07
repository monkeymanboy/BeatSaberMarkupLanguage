using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Strokable : MonoBehaviour
    {
        public Image Image;

        public enum StrokeType
        {
            None,
            Clean,
            Regular,
        }

        public void SetType(StrokeType strokeType)
        {
            if (Image == null)
            {
                return;
            }

            switch (strokeType)
            {
                case StrokeType.None:
                    Image.enabled = false;
                    break;
                case StrokeType.Clean:
                    Image.enabled = true;
                    Image.sprite = Utilities.FindSpriteCached("RoundRectSmallStroke");
                    break;
                case StrokeType.Regular:
                    Image.enabled = true;
                    Image.sprite = Utilities.FindSpriteCached("RoundRectBigStroke");
                    break;
            }
        }

        public void SetColor(string strokeColor)
        {
            if (Image == null)
            {
                return;
            }

            Image.color = Parse.Color(strokeColor);
            Image.enabled = true;
        }
    }
}
