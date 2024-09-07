using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSetting : MonoBehaviour
    {
        public BSMLAction Formatter;
        public BSMLAction OnChange;
        public BSMLValue AssociatedValue;
        public bool UpdateOnChange;

        public abstract void Setup();

        public abstract void ApplyValue();

        public abstract void ReceiveValue();
    }
}
