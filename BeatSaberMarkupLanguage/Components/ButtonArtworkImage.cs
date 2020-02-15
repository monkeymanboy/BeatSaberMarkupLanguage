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
                image = GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "BGArtwork");
            if (image == null)
                throw new Exception("Unable to find BG artwork image!");

            Utilities.GetData(path, (byte[] data) =>
            {
                image.sprite = Utilities.LoadSpriteRaw(data);
            });
        }
    }
}