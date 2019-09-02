using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;
using static HMUI.TableView;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(CustomListTableData))]
    public class CustomListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "selectCell", new[]{ "select-cell" } },
            { "visibleCells", new[]{ "visible-cells"} },
            { "cellSize", new[]{ "cell-size"} },
            { "id", new[]{ "id" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "expandCell", new[] { "expand-cell" } },
            { "listStyle", new[] { "list-style" } },
            { "listDirection", new[] { "list-direction" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            CustomListTableData tableData = obj as CustomListTableData;
            if (data.ContainsKey("selectCell"))
            {
                tableData.tableView.didSelectCellWithIdxEvent += delegate(HMUI.TableView table, int index) {
                    if (!parserParams.actions.ContainsKey(data["selectCell"]))
                        throw new Exception("select-cell action '" + data["onClick"] + "' not found");
                    parserParams.actions[data["selectCell"]].Invoke(table, index);
                };
            }
            if (data.ContainsKey("listDirection"))
                tableData.tableView.SetPrivateField("_tableType", (TableType)Enum.Parse(typeof(TableType), data["listDirection"]));
            if (data.ContainsKey("listStyle"))
                tableData.Style = (ListStyle) Enum.Parse(typeof(ListStyle), data["listStyle"]);
            if (data.ContainsKey("cellSize"))
                tableData.cellSize = Parse.Float(data["cellSize"]);
            if (data.ContainsKey("expandCell"))
                tableData.expandCell = Parse.Bool(data["expandCell"]);
            switch (tableData.tableView.tableType)
            {
                case TableType.Vertical:
                    (obj.gameObject.transform as RectTransform).sizeDelta = new Vector2(data.ContainsKey("listWidth") ? Parse.Float(data["listWidth"]) : 60, tableData.cellSize * (data.ContainsKey("visibleCells") ? Parse.Float(data["visibleCells"]) : 7));
                    tableData.tableView.contentTransform.anchorMin = new Vector2(0, 1);
                    break;
                case TableType.Horizontal:
                    (obj.gameObject.transform as RectTransform).sizeDelta = new Vector2(tableData.cellSize * (data.ContainsKey("visibleCells") ? Parse.Float(data["visibleCells"]) : 4), data.ContainsKey("listHeight") ? Parse.Float(data["listHeight"]) : 40);
                    tableData.tableView.contentTransform.anchorMin = new Vector2(1, 0);
                    break;
            }
            
            obj.gameObject.GetComponent<LayoutElement>().preferredHeight = (obj.gameObject.transform as RectTransform).sizeDelta.y;
            obj.gameObject.GetComponent<LayoutElement>().preferredWidth = (obj.gameObject.transform as RectTransform).sizeDelta.x;
            tableData.tableView.gameObject.SetActive(true);
            if (data.ContainsKey("id"))
            {
                TableViewScroller scroller = tableData.tableView.GetPrivateField<TableViewScroller>("_scroller");
                parserParams.AddEvent(data["id"] + "#PageUp", scroller.PageScrollUp);
                parserParams.AddEvent(data["id"] + "#PageDown", scroller.PageScrollDown);
            }
        }
    }
}
