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
        public TabSelector selector;
    }
}
