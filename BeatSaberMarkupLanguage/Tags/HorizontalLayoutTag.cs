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
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(BSMLParser.SCREEN_WIDTH, BSMLParser.SCREEN_HEIGHT);
            return gameObject;
        }
    }
}
