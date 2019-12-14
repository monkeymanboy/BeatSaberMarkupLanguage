using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class Strokable : MonoBehaviour
    {
        public Image image;

        public void SetType(string strokeType)
        {
            if (image == null)
                return;

            switch (strokeType)
            {
                case "none":
                    image.enabled = false;
                    break;
                case "small":
                    image.enabled = true;
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Last(x => x.name == "RoundRectSmallStroke");
                    break;
                case "big":
                    image.enabled = true;
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Last(x => x.name == "RoundRectBigStroke");
                    break;
            }
        }

        public void SetColor(string strokeColor)
        {
            if (image == null)
                return;

            if (strokeColor != "none")
            {
                if (!ColorUtility.TryParseHtmlString(strokeColor, out Color color))
                    Logger.log.Warn($"Invalid color: {strokeColor}");
                image.color = color;
                image.enabled = true;
            }
            else
            {
                image.enabled = false;
            }
        }
    }
}
