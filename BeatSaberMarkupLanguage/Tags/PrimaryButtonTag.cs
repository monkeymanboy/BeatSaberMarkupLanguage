using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PrimaryButtonTag : ButtonTag
    {
        public override string[] Aliases => new[] { "primary-button", "action-button" };

        public override Button PrefabButton => BeatSaberUI.DiContainer.Resolve<PracticeViewController>()._playButton;

        public override GameObject CreateObject(Transform parent)
        {
            return base.CreateObject(parent).AddComponent<LayoutElement>().gameObject;
        }
    }
}
