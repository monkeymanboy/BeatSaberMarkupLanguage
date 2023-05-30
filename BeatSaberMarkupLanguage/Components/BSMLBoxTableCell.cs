using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLBoxTableCell : TableCell
    {
        private ImageView _coverImage;

        private ImageView _selectionImage;

        private Color _selectedColor0 = new Color(0f, 64f / 85f, 1f, 1f);

        private Color _selectedColor1 = new Color(0f, 64f / 85f, 1f, 0f);

        private Color _highlightedColor0 = new Color(0f, 64f / 85f, 1f, 1f);

        private Color _highlightedColor1 = new Color(0f, 64f / 85f, 1f, 0f);

        internal void SetComponents(ImageView coverImage, ImageView selectionImage)
        {
            _coverImage = coverImage;
            _selectionImage = selectionImage;
        }

        public void SetData(Sprite coverSprite)
        {
            if (_coverImage != null)
            {
                _coverImage.sprite = coverSprite;
            }
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
            if (base.selected || base.highlighted)
            {
                _selectionImage.enabled = true;
                _selectionImage.color0 = (base.highlighted ? _highlightedColor0 : _selectedColor0);
                _selectionImage.color1 = (base.highlighted ? _highlightedColor1 : _selectedColor1);
            }
            else
            {
                _selectionImage.enabled = false;
            }
        }
    }
}
