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
            scrollTransform.anchoredPosition = new Vector2(2, 6);
            scrollTransform.sizeDelta = new Vector2(0, -20);
            scrollTransform.gameObject.name = "BSMLScrollableSettingsContainer";
            return content;
        }
    }
}
