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
    [ComponentHandler(typeof(Button))]
    public class ButtonHandler : TypeHandler<Button>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "text", new[]{"text"} },
            { "glowColor", new[]{ "glow-color" } },
            { "onClick", new[]{ "on-click" } },
            { "clickEvent", new[]{ "click-event", "event-click"} },
            { "interactable", new[]{ "interactable" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                Button button = componentType.component as Button;

                Image glowImage = button.gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.gameObject.name == "Glow");
                if (glowImage != null)
                    glowImage.gameObject.SetActive(false);
                Polyglot.LocalizedTextMeshProUGUI localizer = componentType.component.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
                if (localizer != null)
                    GameObject.Destroy(localizer);
                base.HandleType(componentType, parserParams);

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
            catch (Exception ex)
            {
                Logger.log?.Error(ex);
            }
        }

        public override Dictionary<string, Action<Button, string>> Setters => new Dictionary<string, Action<Button, string>>()
        {
            {"text", new Action<Button, string>(SetLabel)},
            {"glowColor", new Action<Button, string>(SetGlow) },
            {"interactable", new Action<Button, string>(SetInteractable) }
        };

        public static void SetLabel(Button button, string value)
        {
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = value;
        }

        public static void SetGlow(Button button, string glowColor)
        {
            Logger.log?.Critical($"Attempting to set glowColor to {glowColor}");
            Image glowImage = button.gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.gameObject.name == "Glow");
            if (glowImage == null)
                return;
            if (glowColor != "none")
            {
                ColorUtility.TryParseHtmlString(glowColor, out Color color);
                glowImage.color = color;
            }
            else
            {
                glowImage.gameObject.SetActive(false);
            }

        }

        public static void SetInteractable(Button button, string flag)
        {
            if (bool.TryParse(flag, out bool interactable))
                button.interactable = interactable;
        }

    }
}