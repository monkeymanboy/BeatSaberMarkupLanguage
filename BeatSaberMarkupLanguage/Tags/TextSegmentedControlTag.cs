using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextSegmentedControlTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "text-segments" };

        protected override PrefabParams CreatePrefab()
        {
            TextSegmentedControl segmentControlTemplate = BeatSaberUI.DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView._beatmapDifficultySegmentedControlController.GetComponent<TextSegmentedControl>();
            TextSegmentedControl textSegmentedControl = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(segmentControlTemplate);
            textSegmentedControl.name = "BSMLTextSegmentedControl";
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            Object.Destroy(textSegmentedControl.GetComponent<BeatmapDifficultySegmentedControlController>());
            return new PrefabParams(textSegmentedControl.gameObject);
        }
    }
}
