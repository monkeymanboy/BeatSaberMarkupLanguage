using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollIndicatorTag : BSMLTag
    {
        private static GameObject verticalScrollTemplate = null;

        public static GameObject VerticalScrollTemplate
        {
            get
            {
                if (verticalScrollTemplate == null)
                {
                    verticalScrollTemplate = BeatSaberUI.DiContainer.Resolve<LevelCollectionNavigationController>()._levelCollectionViewController._levelCollectionTableView.GetComponent<ScrollView>()._verticalScrollIndicator.gameObject;
                }

                return verticalScrollTemplate;
            }
        }

        public override string[] Aliases { get; } = new[] { "vertical-scroll-indicator", "scroll-indicator" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = Object.Instantiate(VerticalScrollTemplate);
            gameObject.SetActive(false);
            gameObject.name = "BSMLScrollIndicator";

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.SetParent(parent, false);

            Object.DestroyImmediate(gameObject.GetComponent<VerticalScrollIndicator>());
            BSMLScrollIndicator indicator = gameObject.AddComponent<BSMLScrollIndicator>();
            indicator.Handle = transform.GetChild(0).GetComponent<RectTransform>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
