using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(PageButton))]
    public class PageButtonHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "direction", new[]{"dir", "direction"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, BSMLAction> actions)
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
        }
        public enum PageButtonDirection
        {
            Up,Down,Left,Right
        }
    }
}
