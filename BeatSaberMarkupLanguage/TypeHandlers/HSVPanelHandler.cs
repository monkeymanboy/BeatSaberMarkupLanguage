using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(HSVPanelController))]
    public class HSVPanelHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "value", new[]{ "value", "color" } },
            { "onChange", new[]{ "on-change" } },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                HSVPanelController hsvPanel = componentType.component as HSVPanelController;
                if (componentType.data.TryGetValue("value", out string colorStr))
                {
                    ColorUtility.TryParseHtmlString(colorStr, out Color color);
                    hsvPanel.color = color;
                }
                if (componentType.data.TryGetValue("onChange", out string onChange))
                {
                    if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                        throw new Exception("on-change '" + onChange + "' not found");
                    hsvPanel.colorDidChangeEvent += delegate (Color c, ColorChangeUIEventType e)
                    {
                        onChangeAction.Invoke(c, e);
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.log?.Error(ex);
            }
        }
    }
}
