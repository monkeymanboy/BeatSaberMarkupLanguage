using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class HSVPanelTag : BSMLTag
    {
        public override string[] Aliases => new[] { "hsv-wheel" };

        public override GameObject CreateObject(Transform parent)
        {
            HSVPanelController controller = Object.Instantiate(Resources.FindObjectsOfTypeAll<HSVPanelController>().Last(x => x.name == "HSVColorPicker"), parent, false);
            if (controller == null)
                return null;
            controller.name = "BSMLHSVPanel";
            controller.gameObject.transform.localPosition = Vector2.zero;
            (controller.gameObject.transform as RectTransform).anchoredPosition = Vector2.zero;
            return controller.gameObject;
        }
    }
}