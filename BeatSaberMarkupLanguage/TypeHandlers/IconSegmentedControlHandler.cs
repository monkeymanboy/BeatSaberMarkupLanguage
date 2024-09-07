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
            IconSegmentedControl iconControl = componentType.Component as IconSegmentedControl;

            if (componentType.Data.TryGetValue("data", out string value))
            {
                if (!parserParams.Values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new ValueNotFoundException(value, parserParams.Host);
                }

                iconControl.SetData((contents.GetValue() as List<IconSegmentedControl.DataItem>).ToArray());
            }

            if (componentType.Data.TryGetValue("selectCell", out string selectCell))
            {
                iconControl.didSelectCellEvent += (SegmentedControl control, int index) =>
                {
                    if (!parserParams.Actions.TryGetValue(selectCell, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(selectCell, parserParams.Host);
                    }

                    action.Invoke(control, index);
                };
            }
        }
    }
}
