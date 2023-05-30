using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class Tab : MonoBehaviour
    {
        private string tabName;
        private string tabKey;

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

        public string TabKey
        {
            get => tabKey;
            set
            {
                tabKey = value;
                selector?.Refresh();
            }
        }

        public TabSelector selector;
    }
}
