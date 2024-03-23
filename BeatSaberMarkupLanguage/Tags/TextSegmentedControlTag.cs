using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextSegmentedControlTag : BSMLTag
    {
        private TextSegmentedControl segmentControlTemplate;

        public override string[] Aliases => new[] { "text-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentControlTemplate == null)
            {
                segmentControlTemplate = DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView._beatmapDifficultySegmentedControlController.GetComponent<TextSegmentedControl>();
            }

            TextSegmentedControl textSegmentedControl = DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(segmentControlTemplate, parent);
            textSegmentedControl.name = "BSMLTextSegmentedControl";
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            Object.Destroy(textSegmentedControl.GetComponent<BeatmapDifficultySegmentedControlController>());
            return textSegmentedControl.gameObject;
        }
    }
}
