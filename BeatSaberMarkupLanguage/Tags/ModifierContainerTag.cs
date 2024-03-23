using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModifierContainerTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "modifier-container" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = new("BSMLModifierContainer")
            {
                layer = 5,
            };

            VerticalLayoutGroup vertical = gameObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(3, 3, 2, 2);
            vertical.childControlHeight = false;
            vertical.childForceExpandHeight = false;

            gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchoredPosition = new Vector2(0, 3);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(54, 0);

            gameObject.AddComponent<LayoutElement>();
            return new PrefabParams(gameObject);
        }
    }
}
