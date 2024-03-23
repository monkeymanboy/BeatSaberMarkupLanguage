using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class VerticalIconSegmentedControlTag : PrefabBSMLTag
    {
        private static IconSegmentedControl prefab;

        public override string[] Aliases => new[] { "vertical-icon-segments" };

        protected override PrefabParams CreatePrefab()
        {
            if (prefab == null)
            {
                prefab = BeatSaberUI.DiContainer.Resolve<PlatformLeaderboardViewController>()._scopeSegmentedControl;
            }

            IconSegmentedControl control = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(prefab);
            control.name = "BSMLVerticalIconSegmentedControl";

            RectTransform rt = control.transform as RectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);

            foreach (Transform transform in control.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            return new PrefabParams(control.gameObject);
        }
    }
}
