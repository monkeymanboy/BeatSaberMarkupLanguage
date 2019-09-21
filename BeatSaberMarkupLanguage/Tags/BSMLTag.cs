using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class BSMLTag
    {
        public abstract string[] Aliases { get; }
        public virtual bool AddChildren { get => true; }

        public abstract GameObject CreateObject(Transform parent);
    }
}
