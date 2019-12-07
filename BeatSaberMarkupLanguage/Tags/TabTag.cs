using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    class TabTag : BackgroundTag
    {
        public override string[] Aliases => new[] { "tab" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = base.CreateObject(parent);
            gameObject.name = "BSMLTab";
            gameObject.AddComponent<Tab>();
            return gameObject;
        }
    }
}
