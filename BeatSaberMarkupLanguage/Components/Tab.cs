using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class Tab : MonoBehaviour
    {
        public TabSelector selector;

        private string tabName;
        private string tabKey;
        private bool isVisible = true;

        public string TabName
        {
            get => tabName;
            set
            {
                tabName = value;
                RefreshSelector();
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                RefreshSelector();
            }
        }

        public string TabKey
        {
            get => tabKey;
            set
            {
                tabKey = value;
                RefreshSelector();
            }
        }

        private void RefreshSelector()
        {
            if (selector != null)
            {
                selector.Refresh();
            }
        }
    }
}
