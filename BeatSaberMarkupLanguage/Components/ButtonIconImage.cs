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

            Utilities.GetData(path, (byte[] data) =>
            {
                image.sprite = Utilities.LoadSpriteRaw(data);
            });
        }
    }
}
