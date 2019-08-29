using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class HorizontalLayoutTag : BSMLTag
    {
        public override string[] Aliases => new[] { "horizontal" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLHorizontalLayoutGroup";
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<HorizontalLayoutGroup>();
            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.AddComponent<Backgroundable>();
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);
            gameObject.AddComponent<LayoutElement>();
            return gameObject;
        }
    }
}
