using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(Button))]
    public class ButtonHandler : TypeHandler<Button>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "onClick", new[] { "on-click" } },
            { "clickEvent", new[] { "click-event", "event-click" } },
        };

        public override Dictionary<string, Action<Button, string>> Setters => new();

        public static void SetInteractable(Button button, string flag)
        {
            button.interactable = Parse.Bool(flag);
        }

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                Button button = componentType.component as Button;

                if (componentType.data.TryGetValue("onClick", out string onClick))
                {
                    button.onClick.AddListener(() =>
                    {
                        if (!parserParams.actions.TryGetValue(onClick, out BSMLAction onClickAction))
                        {
                            throw new Exception("on-click action '" + onClick + "' not found");
                        }

                        onClickAction.Invoke();
                    });
                }

                if (componentType.data.TryGetValue("clickEvent", out string clickEvent))
                {
                    button.onClick.AddListener(() =>
                    {
                        parserParams.EmitEvent(clickEvent);
                    });
                }

                base.HandleType(componentType, parserParams);
            }
            catch (Exception ex)
            {
                Logger.Log?.Error(ex);
            }
        }
    }
}
