using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonArtworkImage : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        public Image Image
        {
            get => image;
            set => image = value;
        }

        public void SetArtwork(string path)
        {
            if (image == null)
            {
                image = GetComponentsInChildren<Image>().Where(x => x.name == "BGArtwork").FirstOrDefault();
            }

            if (image == null)
            {
                throw new BSMLException("Unable to find BG artwork image!");
            }

            image.SetImageAsync(path).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
