using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LoadingIndicatorTag : PrefabBSMLTag
    {
        public override string[] Aliases => new string[] { "loading", "loading-indicator" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject loadingIndicator = Object.Instantiate(BeatSaberUI.DiContainer.Resolve<LevelCollectionNavigationController>()._loadingControl._loadingContainer.transform.Find("LoadingIndicator").gameObject);
            loadingIndicator.name = "BSMLLoadingIndicator";
            loadingIndicator.AddComponent<LayoutElement>();

            return new PrefabParams(loadingIndicator);
        }
    }
}
