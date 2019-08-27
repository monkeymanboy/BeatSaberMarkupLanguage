using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class SettingsContainerTag : BSMLTag
    {
        public override string[] Aliases => new[] { "settings-container" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLSettingsContainer";
            gameObject.transform.SetParent(parent, false);
            VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.spacing = 0.5f;
            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            RectTransform rectTransform = gameObject.transform as RectTransform;
            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
