using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(IconSegmentedControl))]
    public class IconSegmentedControlHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "selectCell", new[] { "select-cell" } },
            { "data", new[] { "contents", "data" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            IconSegmentedControl iconControl = componentType.component as IconSegmentedControl;

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new ValueNotFoundException(value, parserParams.host);
                }

                iconControl.SetData((contents.GetValue() as List<IconSegmentedControl.DataItem>).ToArray());
            }

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                iconControl.didSelectCellEvent += (SegmentedControl control, int index) =>
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(selectCell, parserParams.host);
                    }

                    action.Invoke(control, index);
                };
            }
        }
    }
}
