using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableSettingsContainerTag : ScrollViewTag
    {
        public override string[] Aliases => new[] { "settings-scroll-view", "scrollable-settings-container", "settings-container" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject content = base.CreateObject(parent);
            ExternalComponents externalComponents = content.GetComponent<ExternalComponents>();
            RectTransform scrollTransform = externalComponents.Get<RectTransform>();
            scrollTransform.anchorMin = Vector2.zero;
            scrollTransform.anchorMax = Vector2.one;
            scrollTransform.anchoredPosition = Vector2.zero;
            scrollTransform.sizeDelta = Vector2.zero;
            scrollTransform.gameObject.name = "BSMLScrollableSettingsContainer";
            return content;
        }
    }
}
