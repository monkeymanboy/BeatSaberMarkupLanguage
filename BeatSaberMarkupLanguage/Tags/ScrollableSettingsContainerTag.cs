using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollableSettingsContainerTag : ScrollViewTag
    {
        public override string[] Aliases => new[] { "scrollable-settings-container" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject content = base.CreateObject(parent);
            ExternalComponents externalComponents = content.GetComponent<ExternalComponents>();
            RectTransform scrollTransform = externalComponents.Get<RectTransform>();
            scrollTransform.anchoredPosition = new Vector2(2, 6);
            scrollTransform.sizeDelta = new Vector2(0, -20);
            scrollTransform.gameObject.name = "BSMLScrollableSettingsContainer";
            externalComponents.Get<BSMLScrollView>().ReserveButtonSpace = true;
            return content;
        }
    }
}
