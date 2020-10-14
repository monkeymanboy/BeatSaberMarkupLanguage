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
        public override string[] Aliases => new[] { "tab-select", "tab-selector" };

        public override GameObject CreateObject(Transform parent)
        {
            TextSegmentedControl prefab = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().FirstOrDefault(x => x.transform.parent.name == "PlayerStatisticsViewController" && x.GetField<DiContainer, TextSegmentedControl>("_container") != null);
            TextSegmentedControl textSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            textSegmentedControl.name = "BSMLTabSelector";
            textSegmentedControl.SetField("_container", prefab.GetField<DiContainer, TextSegmentedControl>("_container"));
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach(Transform transform in textSegmentedControl.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }
            textSegmentedControl.gameObject.AddComponent<TabSelector>().textSegmentedControl = textSegmentedControl;
            return textSegmentedControl.gameObject;
        }
    }
}
