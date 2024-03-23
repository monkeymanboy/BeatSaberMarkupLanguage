using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableSettingsContainerTag : ScrollViewTag
    {
        public override string[] Aliases => new[] { "settings-scroll-view", "scrollable-settings-container", "settings-container" };

        protected override PrefabParams CreatePrefab()
        {
            PrefabParams prefab = base.CreatePrefab();
            ExternalComponents externalComponents = prefab.ContainerObject.GetComponent<ExternalComponents>();
            RectTransform scrollTransform = externalComponents.Get<RectTransform>();
            scrollTransform.anchorMin = Vector2.zero;
            scrollTransform.anchorMax = Vector2.one;
            scrollTransform.anchoredPosition = new Vector2(2, 6);
            scrollTransform.sizeDelta = new Vector2(0, -20);
            scrollTransform.gameObject.name = "BSMLScrollableSettingsContainer";
            return prefab;
        }
    }
}
