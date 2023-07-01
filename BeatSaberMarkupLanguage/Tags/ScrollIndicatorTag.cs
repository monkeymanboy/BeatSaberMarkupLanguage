using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollIndicatorTag : BSMLTag
    {
        public static GameObject VerticalScrollTemplate { get; private set; }

        public override string[] Aliases { get; } = new[] { "vertical-scroll-indicator", "scroll-indicator" };

        public override void Setup()
        {
            base.Setup();
            VerticalScrollTemplate = DiContainer.Resolve<GameplaySetupViewController>().GetChildGameObject("PlayerOptions/ScrollBar/VerticalScrollIndicator");
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = Object.Instantiate(VerticalScrollTemplate);
            gameObject.SetActive(false);
            gameObject.name = "BSMLScrollIndicator";

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.SetParent(parent, false);

            Object.DestroyImmediate(gameObject.GetComponent<VerticalScrollIndicator>());
            BSMLScrollIndicator indicator = gameObject.AddComponent<BSMLScrollIndicator>();
            indicator.Handle = transform.GetChild(0).GetComponent<RectTransform>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
