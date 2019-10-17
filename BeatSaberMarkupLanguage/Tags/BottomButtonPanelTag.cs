using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class BottomButtonPanelTag : BSMLTag
    {
        public override string[] Aliases => new[] { "bottom-button-panel" };

        public override GameObject CreateObject(Transform parent)
        {
            RectTransform container = new GameObject().AddComponent<RectTransform>();
            container.name = "BSMLBottomPanelContainer";
            container.SetParent(parent, false);
            container.anchorMin = new Vector2(0, 0);
            container.anchorMax = new Vector2(1, 0);
            container.sizeDelta = new Vector2(0, 0);

            StartMiddleEndButtonsGroup group = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<StartMiddleEndButtonsGroup>().First(x => (x.name == "Buttons")), container, false);
            group.name = "BSMLBottomPanelButtons";
            foreach (Transform transform in group.transform)
                GameObject.Destroy(transform.gameObject);

            return group.gameObject;
        }
    }
}
