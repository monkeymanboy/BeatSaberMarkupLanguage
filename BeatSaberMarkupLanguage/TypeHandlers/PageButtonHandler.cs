using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(PageButton))]
    public class PageButtonHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "direction", new[]{"dir", "direction"} },
            { "buttonWidth", new[]{ "button-width" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if(data.ContainsKey("direction"))
                switch(Enum.Parse(typeof(PageButtonDirection), data["direction"]))
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
                };
            if (data.ContainsKey("buttonWidth"))
            {
                LayoutElement layoutElement = obj.gameObject.GetComponent<LayoutElement>();
                layoutElement.preferredWidth = float.Parse(data["buttonWidth"]);
                (obj.transform.GetChild(0).transform as RectTransform).sizeDelta = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
            }
        }
        public enum PageButtonDirection
        {
            Up,Down,Left,Right
        }
    }
}
