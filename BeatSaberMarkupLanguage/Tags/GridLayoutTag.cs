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
    public class GridLayoutTag : BSMLTag
    {
        public override string[] Aliases => new[] { "grid" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLGridLayoutGroup";
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<GridLayoutGroup>();
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(BSMLParser.SCREEN_WIDTH, BSMLParser.SCREEN_HEIGHT);
            return gameObject;
        }
    }
}
