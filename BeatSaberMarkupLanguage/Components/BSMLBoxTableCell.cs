using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLBoxTableCell : TableCell
    {
        private ImageView coverImage;

        private ImageView selectionImage;

        private Color selectedColor0 = new(0f, 64f / 85f, 1f, 1f);

        private Color selectedColor1 = new(0f, 64f / 85f, 1f, 0f);

        private Color highlightedColor0 = new(0f, 64f / 85f, 1f, 1f);

        private Color highlightedColor1 = new(0f, 64f / 85f, 1f, 1f);

        public void SetData(Sprite coverSprite)
        {
            if (coverImage != null)
            {
                coverImage.sprite = coverSprite;
            }
        }

        internal void SetComponents(ImageView coverImage, ImageView selectionImage)
        {
            this.coverImage = coverImage;
            this.selectionImage = selectionImage;
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
                selectionImage.enabled = true;
                selectionImage.color0 = highlighted ? highlightedColor0 : selectedColor0;
                selectionImage.color1 = highlighted ? highlightedColor1 : selectedColor1;
            }
            else
            {
                selectionImage.enabled = false;
            }
        }
    }
}
