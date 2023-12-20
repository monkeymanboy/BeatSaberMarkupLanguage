using System.Linq;
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
                segmentControlTemplate = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().FirstOrDefault(x => x.transform.parent.name == "PlayerStatisticsViewController" && x._container != null);
            }

            TextSegmentedControl textSegmentedControl = DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(segmentControlTemplate, parent);
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
