using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(CustomListTableData))]
    public class CustomListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "selectCell", new[] { "select-cell" } },
            { "visibleCells", new[] { "visible-cells" } },
            { "cellSize", new[] { "cell-size" } },
            { "id", new[] { "id" } },
            { "data", new[] { "data", "content" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "expandCell", new[] { "expand-cell" } },
            { "listStyle", new[] { "list-style" } },
            { "listDirection", new[] { "list-direction" } },
            { "alignCenter", new[] { "align-to-center" } },
            { "stickScrolling", new[] { "stick-scrolling" } },
            { "showScrollbar", new[] { "show-scrollbar" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            CustomListTableData tableData = componentType.component as CustomListTableData;
            ScrollView scrollView = tableData.tableView._scrollView;

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.tableView.didSelectCellWithIdxEvent += (TableView table, int index) =>
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                    {
                        throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");
                    }

                    action.Invoke(table, index);
                };
            }

            bool verticalList = true;

            if (componentType.data.TryGetValue("listDirection", out string listDirection))
            {
                tableData.tableView._tableType = (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection);
                scrollView._scrollViewDirection = (ScrollView.ScrollViewDirection)Enum.Parse(typeof(ScrollView.ScrollViewDirection), listDirection);
                verticalList = listDirection.ToLower() != "horizontal";
            }

            if (componentType.data.TryGetValue("listStyle", out string listStyle))
            {
                tableData.Style = (ListStyle)Enum.Parse(typeof(ListStyle), listStyle);
            }

            if (componentType.data.TryGetValue("cellSize", out string cellSize))
            {
                tableData.cellSize = Parse.Float(cellSize);
            }

            if (componentType.data.TryGetValue("expandCell", out string expandCell))
            {
                tableData.expandCell = Parse.Bool(expandCell);
            }

            if (componentType.data.TryGetValue("alignCenter", out string alignCenter))
            {
                tableData.tableView._alignToCenter = Parse.Bool(alignCenter);
            }

            // We can only show the scroll bar for vertical lists
            if (verticalList && componentType.data.TryGetValue("showScrollbar", out string showScrollbar))
            {
                if (Parse.Bool(showScrollbar))
                {
                    TextPageScrollView textScrollView = Object.Instantiate(ScrollViewTag.ScrollViewTemplate, componentType.component.transform);

                    Button pageUpButton = textScrollView._pageUpButton;
                    Button pageDownButton = textScrollView._pageDownButton;
                    VerticalScrollIndicator verticalScrollIndicator = textScrollView._verticalScrollIndicator;
                    RectTransform scrollBar = verticalScrollIndicator.transform.parent as RectTransform;

                    scrollView._pageUpButton = pageUpButton;
                    scrollView._pageDownButton = pageDownButton;
                    scrollView._verticalScrollIndicator = verticalScrollIndicator;
                    scrollBar.SetParent(componentType.component.transform);
                    Object.Destroy(textScrollView.gameObject);

                    // Need to adjust scroll bar positioning
                    scrollBar.anchorMin = new Vector2(1, 0);
                    scrollBar.anchorMax = Vector2.one;
                    scrollBar.offsetMin = Vector2.zero;
                    scrollBar.offsetMax = new Vector2(8, 0);
                }
            }

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new Exception("value '" + value + "' not found");
                }

                object tableDataValue = contents.GetValue();
                if (tableDataValue is not List<CustomCellInfo> tableDataList)
                {
                    throw new Exception($"Value '{value}' is not a List<CustomCellInfo>, which is required for custom-list");
                }

                tableData.data = tableDataList;
                tableData.tableView.ReloadData();
            }

            switch (tableData.tableView.tableType)
            {
                case TableView.TableType.Vertical:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    break;
                case TableView.TableType.Horizontal:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40);
                    break;
            }

            componentType.component.gameObject.GetComponent<LayoutElement>().preferredHeight = (componentType.component.gameObject.transform as RectTransform).sizeDelta.y;
            componentType.component.gameObject.GetComponent<LayoutElement>().preferredWidth = (componentType.component.gameObject.transform as RectTransform).sizeDelta.x;

            tableData.tableView.gameObject.SetActive(true);
            tableData.tableView.LazyInit();

            if (componentType.data.TryGetValue("id", out string id))
            {
                parserParams.AddEvent(id + "#PageUp", scrollView.PageUpButtonPressed);
                parserParams.AddEvent(id + "#PageDown", scrollView.PageDownButtonPressed);
            }
        }
    }
}
