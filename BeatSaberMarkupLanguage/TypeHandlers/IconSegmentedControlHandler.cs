using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(IconSegmentedControl))]
    public class IconSegmentedControlHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "selectCell", new[]{ "select-cell" } },
            { "data", new[] { "contents", "data" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            IconSegmentedControl iconControl = componentType.component as IconSegmentedControl;

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                iconControl.SetData((contents.GetValue() as List<IconSegmentedControl.DataItem>).ToArray());
            }

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                iconControl.didSelectCellEvent += (SegmentedControl control, int index) => {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + componentType.data["selectCell"] + "' not found");
                    action.Invoke(control, index);
                };
            }

        }

    }
}
