using BeatSaberMarkupLanguage.Components;
using BS_Utils.Utilities;
using HMUI;
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
            TextSegmentedControl prefab = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().First(x => x.transform.parent.name == "PlayerStatisticsViewController");
            TextSegmentedControl textSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            textSegmentedControl.name = "BSMLTabSelector";
            textSegmentedControl.SetPrivateField("_container", prefab.GetPrivateField<DiContainer>("_container"));
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
