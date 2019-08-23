using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Button))]
    public class ButtonHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "glowColor", new[]{"glow-color"} },
            { "onClick", new[]{"on-click"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, BSMLAction> actions)
        {
            Button button = obj as Button;
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
            if (data.ContainsKey("onClick"))
            {
                button.onClick.AddListener(delegate
                {
                    if (!actions.ContainsKey(data["onClick"]))
                        throw new Exception("on-click action '" + data["onClick"] + "' not found");
                    actions[data["onClick"]].Invoke();
                });
            }
        }
    }
}
