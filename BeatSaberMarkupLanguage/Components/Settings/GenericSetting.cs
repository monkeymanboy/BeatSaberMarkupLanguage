using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSetting : MonoBehaviour
    {
        public BSMLAction Formatter { get; set; }

        public BSMLAction OnChange { get; set; }

        public BSMLValue AssociatedValue { get; set; }

        public bool UpdateOnChange { get; set; }

        public abstract void Setup();

        public abstract void ApplyValue();

        public abstract void ReceiveValue();
    }
}
