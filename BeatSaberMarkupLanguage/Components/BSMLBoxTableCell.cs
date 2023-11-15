using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLBoxTableCell : TableCell
    {
#if GAME_VERSION_1_29_0
        // for compatibility with YUR Fit reflection
        private ImageView _coverImage;
        private ImageView _selectionImage;
#else
        private ImageView coverImage;
        private ImageView selectionImage;
#endif

        private Color selectedColor0 = new(0f, 64f / 85f, 1f, 1f);

        private Color selectedColor1 = new(0f, 64f / 85f, 1f, 0f);

        private Color highlightedColor0 = new(0f, 64f / 85f, 1f, 1f);

        private Color highlightedColor1 = new(0f, 64f / 85f, 1f, 1f);

        public void SetData(Sprite coverSprite)
        {
#if GAME_VERSION_1_29_0
            if (_coverImage != null)
            {
                _coverImage.sprite = coverSprite;
            }
#else
            if (coverImage != null)
            {
                coverImage.sprite = coverSprite;
            }
#endif
        }

        internal void SetComponents(ImageView coverImage, ImageView selectionImage)
        {
#if GAME_VERSION_1_29_0
            this._coverImage = coverImage;
            this._selectionImage = selectionImage;
#else
            this.coverImage = coverImage;
            this.selectionImage = selectionImage;
#endif
        }

        protected override void SelectionDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (selected || highlighted)
            {
#if GAME_VERSION_1_29_0
                _selectionImage.enabled = true;
                _selectionImage.color0 = highlighted ? highlightedColor0 : selectedColor0;
                _selectionImage.color1 = highlighted ? highlightedColor1 : selectedColor1;
#else
                selectionImage.enabled = true;
                selectionImage.color0 = highlighted ? highlightedColor0 : selectedColor0;
                selectionImage.color1 = highlighted ? highlightedColor1 : selectedColor1;
#endif
            }
            else
            {
#if GAME_VERSION_1_29_0
                _selectionImage.enabled = false;
#else
                selectionImage.enabled = false;
#endif
            }
        }
    }
}
