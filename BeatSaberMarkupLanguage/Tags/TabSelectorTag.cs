using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TabSelectorTag : BSMLTag
    {
        private TextSegmentedControl _segmentControlTemplate;

        public override string[] Aliases => new[] { "tab-select", "tab-selector" };

        public override void Setup()
        {
            base.Setup();
            _segmentControlTemplate = DiContainer.Resolve<PlayerStatisticsViewController>().GetComponentOnChild<TextSegmentedControl>("HorizontalTextSegmentedControl");
        }

        public override GameObject CreateObject(Transform parent)
        {
            TextSegmentedControl textSegmentedControl = DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(_segmentControlTemplate, parent);
            textSegmentedControl.name = "BSMLTabSelector";
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            textSegmentedControl.gameObject.AddComponent<TabSelector>().textSegmentedControl = textSegmentedControl;
            return textSegmentedControl.gameObject;
        }
    }
}
