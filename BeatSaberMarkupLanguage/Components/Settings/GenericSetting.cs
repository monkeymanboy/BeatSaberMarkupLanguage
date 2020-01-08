using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericSetting : MonoBehaviour
    {
        public BSMLAction formatter;
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange;

        public abstract void Setup();
        public abstract void ApplyValue();
        public abstract void ReceiveValue();
    }
}
