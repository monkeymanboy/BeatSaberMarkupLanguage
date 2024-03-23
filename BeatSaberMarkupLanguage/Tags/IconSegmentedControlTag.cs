using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class IconSegmentedControlTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "icon-segments" };

        protected override PrefabParams CreatePrefab()
        {
            IconSegmentedControl segmentedControlTemplate = BeatSaberUI.DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView._beatmapCharacteristicSegmentedControlController.GetComponent<IconSegmentedControl>();
            IconSegmentedControl iconSegmentedControl = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(segmentedControlTemplate);
            iconSegmentedControl.name = "BSMLIconSegmentedControl";
            (iconSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in iconSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            Object.Destroy(iconSegmentedControl.GetComponent<BeatmapCharacteristicSegmentedControlController>());
            return new PrefabParams(iconSegmentedControl.gameObject);
        }
    }
}
