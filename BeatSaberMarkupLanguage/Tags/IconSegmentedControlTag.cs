using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class IconSegmentedControlTag : BSMLTag
    {
        private IconSegmentedControl segmentedControlTemplate;

        public override string[] Aliases => new[] { "icon-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentedControlTemplate == null)
                segmentedControlTemplate = Resources.FindObjectsOfTypeAll<IconSegmentedControl>().First(x => x.name == "BeatmapCharacteristicSegmentedControl" && x.GetField<DiContainer, IconSegmentedControl>("_container") != null);
            IconSegmentedControl iconSegmentedControl = Object.Instantiate(segmentedControlTemplate, parent, false);
            iconSegmentedControl.name = "BSMLIconSegmentedControl";
            iconSegmentedControl.SetField("_container", segmentedControlTemplate.GetField<DiContainer, IconSegmentedControl>("_container"));
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
