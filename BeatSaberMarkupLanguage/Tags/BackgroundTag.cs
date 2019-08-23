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
    public class BackgroundTag : BSMLTag
    {
        public override string[] Aliases => new[] { "bg", "background", "div" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLBackground";
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();
            //RectTransform rectTransform = gameObject.transform as RectTransform;
            //rectTransform.sizeDelta = new Vector2(BSMLParser.SCREEN_WIDTH, BSMLParser.SCREEN_HEIGHT);
            return gameObject;
        }
    }
}
