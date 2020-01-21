using BS_Utils.Utilities;
using HMUI;
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
            TextSegmentedControl prefab = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().First(x => x.name == "BeatmapDifficultySegmentedControl" && x.GetPrivateField<DiContainer>("_container") != null);
            TextSegmentedControl textSegmentedControl = MonoBehaviour.Instantiate(prefab, parent, false);
            textSegmentedControl.name = "BSMLTextSegmentedControl";
            textSegmentedControl.SetPrivateField("_container", prefab.GetPrivateField<DiContainer>("_container"));
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
