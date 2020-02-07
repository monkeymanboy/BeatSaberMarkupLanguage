using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class RGBPanelTag : BSMLTag
    {
        public override string[] Aliases => new[] { "rgb-panel", "rgb" };

        public override GameObject CreateObject(Transform parent)
        {
            RGBPanelController controller = Object.Instantiate(Resources.FindObjectsOfTypeAll<RGBPanelController>().Last(x => x.name == "RGBColorPicker"), parent, false);
            if (controller == null)
                return null;
            controller.name = "BSMLRGBPanel";
            controller.gameObject.transform.localPosition = Vector2.zero;
            (controller.gameObject.transform as RectTransform).anchoredPosition = Vector2.zero;
            return controller.gameObject;
        }
    }
}
