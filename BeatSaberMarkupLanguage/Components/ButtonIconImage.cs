using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class ButtonIconImage : MonoBehaviour
    {
        public Image Image;

        internal NoTransitionsButton Button;
        internal GameObject Underline;

        public void SetIcon(string path)
        {
            if (Image == null)
            {
                return;
            }

            Image.SetImageAsync(path).ContinueWith((task) => Logger.Log.Error($"Failed to load image\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        internal void SetSkew(float value)
        {
            if (Image is not ImageView imageView)
            {
                return;
            }

            imageView._skew = value;
            imageView.SetVerticesDirty();
        }

        internal void SetUnderlineActive(bool active)
        {
            if (Underline != null)
            {
                Underline.SetActive(active);
            }
        }

        protected void OnEnable()
        {
            Button.selectionStateDidChangeEvent += OnSelectionStateDidChange;
        }

        protected void OnDisable()
        {
            Button.selectionStateDidChangeEvent -= OnSelectionStateDidChange;
        }

        private void OnSelectionStateDidChange(NoTransitionsButton.SelectionState state)
        {
            Color color = Image.color;
            color.a = state is NoTransitionsButton.SelectionState.Disabled ? 0.25f : 1;
            Image.color = color;
        }
    }
}
