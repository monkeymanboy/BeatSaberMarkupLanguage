using IPA.Config.Data;
using System;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class TextMeshProProxy : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        internal TextPageScrollViewRefresher TextPageScrollViewRefresher;
        public void RefreshScrollView()
        {
            TextPageScrollViewRefresher.Refresh();
        }
    }
}
