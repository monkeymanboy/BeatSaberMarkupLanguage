using HMUI;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class VerticalIconSegmentedControlTag : BSMLTag
    {
        private IconSegmentedControl _prefab;

        public override string[] Aliases => new[] { "vertical-icon-segments" };

        public override void Setup()
        {
            base.Setup();
            _prefab = DiContainer.Resolve<PlatformLeaderboardViewController>()._scopeSegmentedControl;
        }

        public override GameObject CreateObject(Transform parent)
        {
            IconSegmentedControl control = DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(_prefab, parent);
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
