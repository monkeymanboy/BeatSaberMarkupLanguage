using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class BSMLTag
    {
        public abstract string[] Aliases { get; }

        public abstract GameObject CreateObject(Transform parent);
    }
}
