using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextSegmentedControl))]
    public class TextSegmentedControlHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "selectCell", new[]{ "select-cell" } },
            { "data", new[] { "contents", "data" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            TextSegmentedControl textControl = componentType.component as TextSegmentedControl;

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                textControl.SetTexts((contents.GetValue() as IEnumerable).Cast<object>().Select(x => x.ToString()).ToArray());
            }

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                textControl.didSelectCellEvent += (SegmentedControl control, int index) =>
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + componentType.data["selectCell"] + "' not found");
                    action.Invoke(control, index);
                };
            }

        }

    }
}
