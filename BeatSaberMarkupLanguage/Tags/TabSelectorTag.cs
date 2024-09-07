using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TabSelectorTag : BSMLTag
    {
        private TextSegmentedControl segmentControlTemplate;

        public override string[] Aliases => new[] { "tab-select", "tab-selector" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentControlTemplate == null)
            {
                segmentControlTemplate = DiContainer.Resolve<PlayerStatisticsViewController>()._statsScopeSegmentedControl;
            }

            TextSegmentedControl textSegmentedControl = DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(segmentControlTemplate, parent);
            textSegmentedControl.name = "BSMLTabSelector";
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            textSegmentedControl.gameObject.AddComponent<TabSelector>().TextSegmentedControl = textSegmentedControl;
            return textSegmentedControl.gameObject;
        }
    }
}
