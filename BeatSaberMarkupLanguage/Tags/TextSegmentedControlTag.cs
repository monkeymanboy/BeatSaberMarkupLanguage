using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    class TextSegmentedControlTag : BSMLTag
    {
        public override string[] Aliases => new[] { "text-segments" };

        public override GameObject CreateObject(Transform parent)
        {
            TextSegmentedControl prefab = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().First(x => x.name == "BeatmapDifficultySegmentedControl" && x.GetField<DiContainer, TextSegmentedControl>("_container") != null);
            TextSegmentedControl textSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            textSegmentedControl.name = "BSMLTextSegmentedControl";
            textSegmentedControl.SetField("_container", prefab.GetField<DiContainer, TextSegmentedControl>("_container"));
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }
            MonoBehaviour.Destroy(textSegmentedControl.GetComponent<BeatmapDifficultySegmentedControlController>());
            return textSegmentedControl.gameObject;
        }
    }
}
