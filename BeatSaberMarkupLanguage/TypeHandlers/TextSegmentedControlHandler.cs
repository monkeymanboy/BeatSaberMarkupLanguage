using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextSegmentedControl))]
    public class TextSegmentedControlHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "selectCell", new[] { "select-cell" } },
            { "data", new[] { "contents", "data" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            TextSegmentedControl textControl = componentType.Component as TextSegmentedControl;

            if (componentType.Data.TryGetValue("data", out string value))
            {
                if (!parserParams.Values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new ValueNotFoundException(value, parserParams.Host);
                }

                textControl.SetTexts((contents.GetValue() as IEnumerable).Cast<object>().Select(x => x.ToString()).ToArray());
            }

            if (componentType.Data.TryGetValue("selectCell", out string selectCell))
            {
                textControl.didSelectCellEvent += (SegmentedControl control, int index) =>
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
