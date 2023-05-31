using System.Linq;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class VerticalIconSegmentedControlTag : BSMLTag
    {
        private static IconSegmentedControl prefab;

        public override string[] Aliases => new[] { "vertical-icon-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            if (prefab == null)
            {
                PlatformLeaderboardViewController vc = Resources.FindObjectsOfTypeAll<PlatformLeaderboardViewController>().First();
                prefab = vc._scopeSegmentedControl;
            }

            IconSegmentedControl control = DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(prefab, parent);
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

            return control.gameObject;
        }
    }
}
