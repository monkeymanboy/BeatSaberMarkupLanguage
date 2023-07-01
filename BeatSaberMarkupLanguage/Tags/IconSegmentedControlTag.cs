using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class IconSegmentedControlTag : BSMLTag
    {
        private IconSegmentedControl _segmentedControlTemplate;

        public override string[] Aliases => new[] { "icon-segments" };

        public override void Setup()
        {
            base.Setup();
            _segmentedControlTemplate = DiContainer.Resolve<StandardLevelDetailViewController>().GetComponentOnChild<IconSegmentedControl>("LevelDetail/BeatmapCharacteristic/BeatmapCharacteristicSegmentedControl");
        }

        public override GameObject CreateObject(Transform parent)
        {
            IconSegmentedControl iconSegmentedControl = DiContainer.InstantiatePrefabForComponent<IconSegmentedControl>(_segmentedControlTemplate, parent);
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
