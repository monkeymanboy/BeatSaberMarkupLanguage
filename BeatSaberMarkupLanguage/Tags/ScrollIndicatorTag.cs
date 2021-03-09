using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ScrollIndicatorTag : BSMLTag
    {
        public override string[] Aliases { get; } = new[] { "vertical-scroll-indicator", "scroll-indicator" };

        private static GameObject _verticalScrollTemplate = null;
        public static GameObject VerticalScrollTemplate
        {
            get
            {
                if (_verticalScrollTemplate == null)
                    _verticalScrollTemplate = Resources.FindObjectsOfTypeAll<VerticalScrollIndicator>().First().gameObject;

                return _verticalScrollTemplate;
            }
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
