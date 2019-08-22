using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Button))]
    public class ButtonHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "glowColor", new[]{"glow-color"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data)
        {
            Polyglot.LocalizedTextMeshProUGUI localizer = obj.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);
            TextMeshProUGUI label = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && data.ContainsKey("text"))
                label.text = data["text"];
            Image glowImage = obj.gameObject.GetComponentsInChildren<Image>().First(x => x.gameObject.name == "Glow");
            if (data.ContainsKey("glowColor") && data["glowColor"] != "none")
            {
                ColorUtility.TryParseHtmlString(data["glowColor"], out Color color);
                glowImage.color = color;
            } else
            {
                glowImage.gameObject.SetActive(false);
            }
        }
    }
}
