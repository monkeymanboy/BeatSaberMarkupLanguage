using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonArtworkImage : MonoBehaviour
    {
        public Image Image;

        public void SetArtwork(string path)
        {
            if (Image == null)
            {
                Image = GetComponentsInChildren<Image>().Where(x => x.name == "BGArtwork").FirstOrDefault();
            }

            if (Image == null)
            {
                throw new BSMLException("Unable to find BG artwork image!");
            }

            Image.SetImageAsync(path).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
