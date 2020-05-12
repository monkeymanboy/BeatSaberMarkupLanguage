using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
