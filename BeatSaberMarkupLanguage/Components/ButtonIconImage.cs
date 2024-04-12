using System.Globalization;
using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonIconImage : MonoBehaviour
    {
        public Image image;

        internal float defaultSkew;
        internal NoTransitionsButton button;

        public void SetIcon(string path)
        {
            if (image == null)
            {
                return;
            }

            image.SetImageAsync(path).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        internal void SetSkew(string value)
        {
            if (image is not ImageView imageView)
            {
                return;
            }

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
            {
                imageView._skew = num;
            }
            else if (bool.TryParse(value, out bool result) && result)
            {
                imageView._skew = defaultSkew;
            }

            imageView.SetVerticesDirty();
        }

        private void OnEnable()
        {
            button.selectionStateDidChangeEvent += OnSelectionStateDidChange;
        }

        private void OnDisable()
        {
            button.selectionStateDidChangeEvent -= OnSelectionStateDidChange;
        }

        private void OnSelectionStateDidChange(NoTransitionsButton.SelectionState state)
        {
            Color color = image.color;
            color.a = state is NoTransitionsButton.SelectionState.Disabled ? 0.25f : 1;
            image.color = color;
        }
    }
}
