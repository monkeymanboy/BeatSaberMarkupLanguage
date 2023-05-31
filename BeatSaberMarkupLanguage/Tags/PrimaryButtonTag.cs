using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PrimaryButtonTag : ButtonTag
    {
        public override string[] Aliases => new[] { "primary-button", "action-button" };

        public override string PrefabButton => "PlayButton";

        public override GameObject CreateObject(Transform parent)
        {
            return base.CreateObject(parent).AddComponent<LayoutElement>().gameObject;
        }
    }
}
