using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(PageButton))]
    public class PageButtonHandler : TypeHandler
    {
        public enum PageButtonDirection
        {
            Up, Down, Left, Right
        }

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "direction", new[]{"dir", "direction"} },
            { "buttonWidth", new[]{ "button-width" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (data.TryGetValue("direction", out string direction))
            {
                switch (Enum.Parse(typeof(PageButtonDirection), direction))
                {
                    case PageButtonDirection.Up:
                        obj.transform.localRotation = Quaternion.Euler(0, 0, -180);
                        break;
                    case PageButtonDirection.Down:
                        obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case PageButtonDirection.Left:
                        obj.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        break;
                    case PageButtonDirection.Right:
                        obj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        break;
                }
            }

            if (data.TryGetValue("buttonWidth", out string buttonWidth))
            {
                LayoutElement layoutElement = obj.gameObject.GetComponent<LayoutElement>();
                layoutElement.preferredWidth = Parse.Float(buttonWidth);
                (obj.transform.GetChild(0).transform as RectTransform).sizeDelta = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
            }
        }
    }
}
