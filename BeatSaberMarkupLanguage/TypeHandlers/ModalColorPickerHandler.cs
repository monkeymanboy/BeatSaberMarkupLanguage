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
                ModalColorPicker colorPicker = componentType.component as ModalColorPicker;
                if (componentType.data.TryGetValue("value", out string value))
                {
                    if (!parserParams.values.TryGetValue(value, out BSMLValue associatedValue))
                    {
                        throw new ValueNotFoundException(value, parserParams.host);
                    }

                    colorPicker.associatedValue = associatedValue;
                }

                if (componentType.data.TryGetValue("onCancel", out string onCancel))
                {
                    if (!parserParams.actions.TryGetValue(onCancel, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onCancel, parserParams.host);
                    }

                    colorPicker.onCancel = action;
                }

                if (componentType.data.TryGetValue("onDone", out string onDone))
                {
                    if (!parserParams.actions.TryGetValue(onDone, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onDone, parserParams.host);
                    }

                    colorPicker.onDone = action;
                }

                if (componentType.data.TryGetValue("onChange", out string onChange))
                {
                    if (!parserParams.actions.TryGetValue(onChange, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(onChange, parserParams.host);
                    }

                    colorPicker.onChange = action;
                }
            }
            catch (Exception ex)
            {
                Logger.Log?.Error(ex);
            }
        }
    }
}
