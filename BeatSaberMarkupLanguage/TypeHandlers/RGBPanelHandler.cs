using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(RGBPanelController))]
    public class RGBPanelHandler : TypeHandler
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
                RGBPanelController rgbPanel = componentType.component as RGBPanelController;
                if (componentType.data.TryGetValue("value", out string colorStr))
                {
                    ColorUtility.TryParseHtmlString(colorStr, out Color color);
                    rgbPanel.color = color;
                    rgbPanel.RefreshSlidersColors();
                    rgbPanel.RefreshSlidersValues();
                }
                if (componentType.data.TryGetValue("onChange", out string onChange))
                {
                    if (!parserParams.actions.TryGetValue(onChange, out BSMLAction onChangeAction))
                        throw new Exception("on-change '" + onChange + "' not found");
                    rgbPanel.colorDidChangeEvent += delegate (Color c, ColorChangeUIEventType e)
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
