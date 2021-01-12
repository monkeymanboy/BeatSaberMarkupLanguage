using Polyglot;
using TMPro;

namespace BeatSaberMarkupLanguage.Components
{
    public class LocalizableText : LocalizedTextMeshProUGUI
    {
        public TextMeshProUGUI TextMesh
        {
            get => localizedComponent;
            set => localizedComponent = value;
        }
    }
}
