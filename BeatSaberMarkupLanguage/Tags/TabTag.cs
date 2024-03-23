using BeatSaberMarkupLanguage.Components;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TabTag : BackgroundTag
    {
        public override string[] Aliases => new[] { "tab" };

        protected override PrefabParams CreatePrefab()
        {
            PrefabParams prefab = base.CreatePrefab();
            prefab.RootObject.name = "BSMLTab";
            prefab.ContainerObject.AddComponent<Tab>();
            return prefab;
        }
    }
}
