using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonArtworkImage : MonoBehaviour
    {
        public Image image;

        public void SetArtwork(string path)
        {
            if (image == null)
            {
                image = GetComponentsInChildren<Image>().Where(x => x.name == "BGArtwork").FirstOrDefault();
            }

            if (image == null)
            {
                throw new Exception("Unable to find BG artwork image!");
            }

            image.SetImage(path);
        }
    }
}
