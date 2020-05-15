using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;
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
            { "data", new[] { "data", "content" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "expandCell", new[] { "expand-cell" } },
            { "listStyle", new[] { "list-style" } },
            { "listDirection", new[] { "list-direction" } },
            { "alignCenter", new[] { "align-to-center" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            CustomListTableData tableData = componentType.component as CustomListTableData;
            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.tableView.didSelectCellWithIdxEvent += delegate (TableView table, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");

                    action.Invoke(table, index);
                };
            }

            if (componentType.data.TryGetValue("listDirection", out string listDirection))
                tableData.tableView.SetField<TableView, TableType>("_tableType", (TableType)Enum.Parse(typeof(TableType), listDirection));

            if (componentType.data.TryGetValue("listStyle", out string listStyle))
                tableData.Style = (ListStyle)Enum.Parse(typeof(ListStyle), listStyle);

            if (componentType.data.TryGetValue("cellSize", out string cellSize))
                tableData.cellSize = Parse.Float(cellSize);

            if (componentType.data.TryGetValue("expandCell", out string expandCell))
                tableData.expandCell = Parse.Bool(expandCell);

            if (componentType.data.TryGetValue("alignCenter", out string alignCenter))
                tableData.tableView.SetField<TableView, bool>("_alignToCenter", Parse.Bool(alignCenter));


            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                tableData.data = contents.GetValue() as List<CustomCellInfo>;
                tableData.tableView.ReloadData();
            }

            switch (tableData.tableView.tableType)
            {
                case TableType.Vertical:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    tableData.tableView.contentTransform.anchorMin = new Vector2(0, 1);
                    break;
                case TableType.Horizontal:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40);
                    break;
            }

            componentType.component.gameObject.GetComponent<LayoutElement>().preferredHeight = (componentType.component.gameObject.transform as RectTransform).sizeDelta.y;
            componentType.component.gameObject.GetComponent<LayoutElement>().preferredWidth = (componentType.component.gameObject.transform as RectTransform).sizeDelta.x;
            tableData.tableView.gameObject.SetActive(true);

            if (componentType.data.TryGetValue("id", out string id))
            {
                TableViewScroller scroller = tableData.tableView.GetField<TableViewScroller, TableView>("_scroller");
                parserParams.AddEvent(id + "#PageUp", scroller.PageScrollUp);
                parserParams.AddEvent(id + "#PageDown", scroller.PageScrollDown);
            }
        }
    }
}
