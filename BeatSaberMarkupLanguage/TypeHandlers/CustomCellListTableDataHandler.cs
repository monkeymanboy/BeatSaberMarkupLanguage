using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HMUI.TableView;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(CustomCellListTableData))]
    public class CustomCellListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "selectCell", new[]{ "select-cell" } },
            { "visibleCells", new[]{ "visible-cells"} },
            { "cellSize", new[]{ "cell-size"} },
            { "id", new[]{ "id" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "listDirection", new[] { "list-direction" } },
            { "data", new[] { "contents", "data" } },
            { "cellClickable", new[] { "clickable-cells" } },
            { "cellTemplate", new[] { "_children" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            CustomCellListTableData tableData = obj as CustomCellListTableData;
            if (data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.tableView.didSelectCellWithIdxEvent += delegate (TableView table, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + data["onClick"] + "' not found");

                    action.Invoke(table, (table.dataSource as CustomCellListTableData).data[index]);
                };
            }

            if (data.TryGetValue("listDirection", out string listDirection))
                tableData.tableView.SetPrivateField("_tableType", (TableType)Enum.Parse(typeof(TableType), listDirection));

            if (data.TryGetValue("cellSize", out string cellSize))
                tableData.cellSize = Parse.Float(cellSize);

            if (data.TryGetValue("cellTemplate", out string cellTemplate))
                tableData.cellTemplate = "<bg>"+cellTemplate+"</bg>";

            if (data.TryGetValue("cellClickable", out string cellClickable))
                tableData.clickableCells = Parse.Bool(cellClickable);

            if (data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                tableData.data = contents.GetValue() as List<object>;
                tableData.tableView.ReloadData();
            }

            switch (tableData.tableView.tableType)
            {
                case TableType.Vertical:
                    (obj.gameObject.transform as RectTransform).sizeDelta = new Vector2(data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    tableData.tableView.contentTransform.anchorMin = new Vector2(0, 1);
                    break;
                case TableType.Horizontal:
                    (obj.gameObject.transform as RectTransform).sizeDelta = new Vector2(tableData.cellSize * (data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40);
                    tableData.tableView.contentTransform.anchorMin = new Vector2(1, 0);
                    break;
            }

            obj.gameObject.GetComponent<LayoutElement>().preferredHeight = (obj.gameObject.transform as RectTransform).sizeDelta.y;
            obj.gameObject.GetComponent<LayoutElement>().preferredWidth = (obj.gameObject.transform as RectTransform).sizeDelta.x;
            tableData.tableView.gameObject.SetActive(true);

            if (data.TryGetValue("id", out string id))
            {
                TableViewScroller scroller = tableData.tableView.GetPrivateField<TableViewScroller>("_scroller");
                parserParams.AddEvent(id + "#PageUp", scroller.PageScrollUp);
                parserParams.AddEvent(id + "#PageDown", scroller.PageScrollDown);
            }
        }
    }
}
