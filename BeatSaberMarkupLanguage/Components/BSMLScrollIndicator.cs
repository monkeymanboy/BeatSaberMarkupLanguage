using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLScrollIndicator : VerticalScrollIndicator
    {
        public RectTransform Handle
        {
            get => _handle;
            set => _handle = value;
        }
    }
}
