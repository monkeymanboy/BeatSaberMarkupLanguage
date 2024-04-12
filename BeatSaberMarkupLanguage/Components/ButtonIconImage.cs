using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonIconImage : MonoBehaviour
    {
        public Image image;

        internal NoTransitionsButton button;
        internal GameObject underline;

        public void SetIcon(string path)
        {
            if (image == null)
            {
                return;
            }

            image.SetImageAsync(path).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        internal void SetSkew(float value)
        {
            if (image is not ImageView imageView)
            {
                return;
            }

            imageView._skew = value;
            imageView.SetVerticesDirty();
        }

        internal void SetUnderlineActive(bool active)
        {
            if (underline != null)
            {
                underline.SetActive(active);
            }
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
