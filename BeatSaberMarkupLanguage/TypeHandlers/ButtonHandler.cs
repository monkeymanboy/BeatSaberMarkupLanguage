using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
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
            { "glowColor", new[]{"glow-color"} },
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            Button button = obj as Button;
            Polyglot.LocalizedTextMeshProUGUI localizer = obj.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);

            TextMeshProUGUI label = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && data.ContainsKey("text"))
                label.text = data["text"];

            Image glowImage = obj.gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.gameObject.name == "Glow");
            if (glowImage != null)
            {
                if (data.ContainsKey("glowColor") && data["glowColor"] != "none")
                {
                    ColorUtility.TryParseHtmlString(data["glowColor"], out Color color);
                    glowImage.color = color;
                }
                else
                {
                    glowImage.gameObject.SetActive(false);
                }
            }

            if (data.ContainsKey("onClick"))
            {
                button.onClick.AddListener(delegate
                {
                    if (!parserParams.actions.ContainsKey(data["onClick"]))
                        throw new Exception("on-click action '" + data["onClick"] + "' not found");

                    parserParams.actions[data["onClick"]].Invoke();
                });
            }

            if (data.ContainsKey("clickEvent"))
            {
                button.onClick.AddListener(delegate
                {
                    parserParams.EmitEvent(data["clickEvent"]);
                });
            }
        }
    }
}
