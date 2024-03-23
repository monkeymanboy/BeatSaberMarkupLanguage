using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PrimaryButtonTag : ButtonTag
    {
        public override string[] Aliases => new[] { "primary-button", "action-button" };

        public override Button PrefabButton => BeatSaberUI.DiContainer.Resolve<PracticeViewController>()._playButton;

        protected override PrefabParams CreatePrefab()
        {
            PrefabParams prefab = base.CreatePrefab();
            prefab.ContainerObject.AddComponent<LayoutElement>();
            return prefab;
        }
    }
}
