using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TabSelectorTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "tab-select", "tab-selector" };

        protected override PrefabParams CreatePrefab()
        {
            TextSegmentedControl segmentControlTemplate = BeatSaberUI.DiContainer.Resolve<PlayerStatisticsViewController>().GetComponentInChildren<TextSegmentedControl>();
            TextSegmentedControl textSegmentedControl = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(segmentControlTemplate);
            textSegmentedControl.name = "BSMLTabSelector";
            ((RectTransform)textSegmentedControl.transform).anchoredPosition = new Vector2(0, 0);

            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            textSegmentedControl.gameObject.AddComponent<TabSelector>().textSegmentedControl = textSegmentedControl;
            return new PrefabParams(textSegmentedControl.gameObject);
        }
    }
}
