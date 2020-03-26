using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    class IconSegmentedControlTag : BSMLTag
    {
        public override string[] Aliases => new[] { "icon-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            IconSegmentedControl prefab = Resources.FindObjectsOfTypeAll<IconSegmentedControl>().First(x => x.name == "BeatmapCharacteristicSegmentedControl" && x.GetField<DiContainer, IconSegmentedControl>("_container") != null);
            IconSegmentedControl iconSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            iconSegmentedControl.name = "BSMLIconSegmentedControl";
            iconSegmentedControl.SetField("_container", prefab.GetField<DiContainer, IconSegmentedControl>("_container"));
            (iconSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in iconSegmentedControl.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }
            MonoBehaviour.Destroy(iconSegmentedControl.GetComponent<BeatmapCharacteristicSegmentedControlController>());
            return iconSegmentedControl.gameObject;
        }
    }
}
