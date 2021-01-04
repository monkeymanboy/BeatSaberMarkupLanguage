using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class Tab : MonoBehaviour
    {
        private string tabName;
        public string TabName
        {
            get => tabName;
            set
            {
                tabName = value;
                selector?.Refresh();
            }
        }
        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                selector?.Refresh();
            }
        }
        public TabSelector selector;
    }
}
