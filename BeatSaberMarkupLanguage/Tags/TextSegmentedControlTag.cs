using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextSegmentedControlTag : BSMLTag
    {
        private TextSegmentedControl segmentControlTemplate;

        public override string[] Aliases => new[] { "text-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentControlTemplate == null)
                segmentControlTemplate = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().First(x => x.name == "BeatmapDifficultySegmentedControl" && x.GetField<DiContainer, TextSegmentedControl>("_container") != null);
            TextSegmentedControl textSegmentedControl = Object.Instantiate(segmentControlTemplate, parent, false);
            textSegmentedControl.name = "BSMLTextSegmentedControl";
            textSegmentedControl.SetField("_container", segmentControlTemplate.GetField<DiContainer, TextSegmentedControl>("_container"));
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
