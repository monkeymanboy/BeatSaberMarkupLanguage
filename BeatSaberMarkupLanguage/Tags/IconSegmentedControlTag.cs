using BS_Utils.Utilities;
using HMUI;
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
            IconSegmentedControl prefab = Resources.FindObjectsOfTypeAll<IconSegmentedControl>().First(x => x.name == "BeatmapCharacteristicSegmentedControl");
            IconSegmentedControl iconSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            iconSegmentedControl.name = "BSMLIconSegmentedControl";
            iconSegmentedControl.SetPrivateField("_container", Resources.FindObjectsOfTypeAll<IconSegmentedControl>().First(x => x.GetPrivateField<DiContainer>("_container") != null).GetPrivateField<DiContainer>("_container"));
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
