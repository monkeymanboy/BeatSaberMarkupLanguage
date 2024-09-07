using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ModalColorPicker))]
    public class ModalColorPickerHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "value", new[] { "value" } },
            { "onCancel", new[] { "on-cancel" } },
            { "onDone", new[] { "on-done" } },
            { "onChange", new[] { "color-change" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                ModalColorPicker colorPicker = componentType.Component as ModalColorPicker;
                if (componentType.Data.TryGetValue("value", out string value))
                {
                    if (!parserParams.Values.TryGetValue(value, out BSMLValue associatedValue))
                    {
                        throw new ValueNotFoundException(value, parserParams.Host);
                    }

                    colorPicker.AssociatedValue = associatedValue;
                }

                if (componentType.Data.TryGetValue("onCancel", out string onCancel))
                {
                    if (!parserParams.Actions.TryGetValue(onCancel, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onCancel, parserParams.Host);
                    }

                    colorPicker.OnCancel = action;
                }

                if (componentType.Data.TryGetValue("onDone", out string onDone))
                {
                    if (!parserParams.Actions.TryGetValue(onDone, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onDone, parserParams.Host);
                    }

                    colorPicker.OnDone = action;
                }

                if (componentType.Data.TryGetValue("onChange", out string onChange))
                {
                    if (!parserParams.Actions.TryGetValue(onChange, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onChange, parserParams.Host);
                    }

                    colorPicker.OnChange = action;
                }
            }
            catch (Exception ex)
            {
                Logger.Log?.Error(ex);
            }
        }
    }
}
