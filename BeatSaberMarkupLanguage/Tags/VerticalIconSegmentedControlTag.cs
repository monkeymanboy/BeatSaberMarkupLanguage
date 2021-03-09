using System.Linq;
using UnityEngine;
using HMUI;
using IPA.Utilities;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class VerticalIconSegmentedControlTag : BSMLTag
    {
        public override string[] Aliases => new[] { "vertical-icon-segments" };

        private static IconSegmentedControl prefab;

        public override GameObject CreateObject(Transform parent)
        {
            if (prefab == null)
            {
                PlatformLeaderboardViewController vc = Resources.FindObjectsOfTypeAll<PlatformLeaderboardViewController>().First();
                prefab = vc.GetField<IconSegmentedControl, PlatformLeaderboardViewController>("_scopeSegmentedControl");
            }

            IconSegmentedControl control = Object.Instantiate(prefab, parent, false);
            control.name = "BSMLVerticalIconSegmentedControl";
            control.SetField("_container", prefab.GetField<DiContainer, IconSegmentedControl>("_container"));

            RectTransform rt = control.transform as RectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);

            foreach (Transform transform in control.transform)
                Object.Destroy(transform.gameObject);

            return control.gameObject;
        }
    }
}
