using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public class ButtonHandlerOld
    {
        public Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "glowColor", new[]{"glow-color"} },
            { "onClick", new[]{"on-click"} },
            { "clickEvent", new[]{"click-event", "event-click"} },
            { "interactable", new[]{ "interactable" } }
        };

        public Dictionary<string, Action<Button, string>> Setters => new Dictionary<string, Action<Button, string>>()
        {
        };

        public void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            Button button = componentType.component as Button;
            Polyglot.LocalizedTextMeshProUGUI localizer = componentType.component.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);

            TextMeshProUGUI label = componentType.component.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && componentType.data.TryGetValue("text", out string text))
                label.text = text;

            if (componentType.data.TryGetValue("interactable", out string interactableString))
                button.interactable = bool.Parse(interactableString);

            Image glowImage = componentType.component.gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.gameObject.name == "Glow");
            if (glowImage != null)
            {
                if (componentType.data.TryGetValue("glowColor", out string glowColor) && glowColor != "none")
                {
                    ColorUtility.TryParseHtmlString(glowColor, out Color color);
                    glowImage.color = color;
                }
                else
                {
                    glowImage.gameObject.SetActive(false);
                }
            }

            if (componentType.data.TryGetValue("onClick", out string onClick))
            {
                button.onClick.AddListener(delegate
                {
                    if (!parserParams.actions.TryGetValue(onClick, out BSMLAction onClickAction))
                        throw new Exception("on-click action '" + onClick + "' not found");

                    onClickAction.Invoke();
                });
            }

            if (componentType.data.TryGetValue("clickEvent", out string clickEvent))
            {
                button.onClick.AddListener(delegate
                {
                    parserParams.EmitEvent(clickEvent);
                });
            }
        }
    }
}
