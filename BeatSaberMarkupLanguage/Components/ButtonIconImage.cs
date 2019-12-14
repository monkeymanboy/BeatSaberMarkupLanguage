using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonIconImage : MonoBehaviour
    {
        public Image image;

        public void SetIcon(string path)
        {
            if (image == null)
                return;

            image.sprite = Utilities.FindSpriteInAssembly(path);
        }
    }
}
