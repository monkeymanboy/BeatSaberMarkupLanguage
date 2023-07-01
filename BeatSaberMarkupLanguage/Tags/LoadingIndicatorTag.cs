using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LoadingIndicatorTag : BSMLTag
    {
        private GameObject loadingTemplate;

        public override string[] Aliases => new string[] { "loading", "loading-indicator" };

        public override void Setup()
        {
            base.Setup();
            loadingTemplate = DiContainer.Resolve<PlatformLeaderboardViewController>().GetChildGameObject("Container/LeaderboardTableView/LoadingControl/LoadingContainer/LoadingIndicator");
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject loadingIndicator = Object.Instantiate(loadingTemplate, parent, false);
            loadingIndicator.name = "BSMLLoadingIndicator";

            loadingIndicator.AddComponent<LayoutElement>();

            return loadingIndicator;
        }
    }
}
