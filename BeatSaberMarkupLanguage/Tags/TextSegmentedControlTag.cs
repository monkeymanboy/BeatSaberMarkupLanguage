using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextSegmentedControlTag : BSMLTag
    {
        private TextSegmentedControl _segmentControlTemplate;

        public override string[] Aliases => new[] { "text-segments" };

        public override void Setup()
        {
            base.Setup();
            _segmentControlTemplate = DiContainer.Resolve<StandardLevelDetailViewController>().GetComponentOnChild<TextSegmentedControl>("LevelDetail/BeatmapDifficulty/BeatmapDifficultySegmentedControl");
        }

        public override GameObject CreateObject(Transform parent)
        {
            TextSegmentedControl textSegmentedControl = DiContainer.InstantiatePrefabForComponent<TextSegmentedControl>(_segmentControlTemplate, parent);
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
