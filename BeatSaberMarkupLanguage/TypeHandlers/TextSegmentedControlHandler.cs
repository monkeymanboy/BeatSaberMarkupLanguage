using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<object> data;

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            TextSegmentedControl textControl = componentType.component as TextSegmentedControl;

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                data = contents.GetValue() as List<object>;
                textControl.SetTexts(data.Select(x => x.ToString()).ToArray());
            }

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                //If we do not set the data, we will not be able to return the object when the cell is selected, so let's warn the user
                if (data == null)
                {
                    Logger.log.Warn("It is not recommended to use 'select-cell' action without providing data in tag, do it at your own risk!");
                }

                void CellSelected(SegmentedControl control, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + componentType.data["selectCell"] + "' not found");
                    action.Invoke(control, data[index]);
                }

                textControl.didSelectCellEvent -= CellSelected;
                textControl.didSelectCellEvent += CellSelected;
                textControl.SelectCellWithNumber(0);
                CellSelected(textControl, 0);
            }

        }

    }
}
