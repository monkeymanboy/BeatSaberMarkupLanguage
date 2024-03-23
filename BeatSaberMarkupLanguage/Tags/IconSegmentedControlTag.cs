using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class IconSegmentedControlTag : BSMLTag
    {
        private IconSegmentedControl segmentedControlTemplate;

        public override string[] Aliases => new[] { "icon-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentedControlTemplate == null)
            {
                segmentedControlTemplate = DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView._beatmapCharacteristicSegmentedControlController.GetComponent<IconSegmentedControl>();
            }

            IconSegmentedControl iconSegmentedControl = DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(segmentedControlTemplate, parent);
            iconSegmentedControl.name = "BSMLIconSegmentedControl";
            (iconSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in iconSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            Object.Destroy(iconSegmentedControl.GetComponent<BeatmapCharacteristicSegmentedControlController>());
            return iconSegmentedControl.gameObject;
        }
    }
}
