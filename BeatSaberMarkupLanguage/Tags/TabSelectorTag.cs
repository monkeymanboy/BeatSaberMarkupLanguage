using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TabSelectorTag : BSMLTag
    {
        private TextSegmentedControl segmentControlTemplate;

        public override string[] Aliases => new[] { "tab-select", "tab-selector" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentControlTemplate == null)
                segmentControlTemplate = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().Where(x => x.transform.parent.name == "PlayerStatisticsViewController" && x.GetField<DiContainer, TextSegmentedControl>("_container") != null).FirstOrDefault();
            TextSegmentedControl textSegmentedControl = Object.Instantiate(segmentControlTemplate, parent, false);
            textSegmentedControl.name = "BSMLTabSelector";
            textSegmentedControl.SetField("_container", segmentControlTemplate.GetField<DiContainer, TextSegmentedControl>("_container"));
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach(Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }
            textSegmentedControl.gameObject.AddComponent<TabSelector>().textSegmentedControl = textSegmentedControl;
            return textSegmentedControl.gameObject;
        }
    }
}
