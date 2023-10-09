using System;
using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(CustomCellListTableData))]
    public class CustomCellListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "selectCell", new[] { "select-cell" } },
            { "visibleCells", new[] { "visible-cells" } },
            { "cellSize", new[] { "cell-size" } },
            { "id", new[] { "id" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "listDirection", new[] { "list-direction" } },
            { "data", new[] { "contents", "data" } },
            { "cellClickable", new[] { "clickable-cells" } },
            { "cellTemplate", new[] { "_children" } },
            { "alignCenter", new[] { "align-to-center" } },
            { "stickScrolling", new[] { "stick-scrolling" } },
            { "showScrollbar", new[] { "show-scrollbar" } },
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            CustomCellListTableData tableData = componentType.component as CustomCellListTableData;
            ScrollView scrollView = tableData.tableView._scrollView;

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.tableView.didSelectCellWithIdxEvent += (TableView table, int index) =>
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                    {
                        throw new BSMLException($"select-cell action '{selectCell}' not found");
                    }

                    action.Invoke(table, (table.dataSource as CustomCellListTableData).data[index]);
                };
            }

            bool verticalList = true;

            if (componentType.data.TryGetValue("listDirection", out string listDirection))
            {
                tableData.tableView._tableType = (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection);
                scrollView._scrollViewDirection = (ScrollView.ScrollViewDirection)Enum.Parse(typeof(ScrollView.ScrollViewDirection), listDirection);
                verticalList = !listDirection.Equals("horizontal", StringComparison.OrdinalIgnoreCase);
            }

            if (componentType.data.TryGetValue("cellSize", out string cellSize))
            {
                tableData.cellSize = Parse.Float(cellSize);
            }

            if (componentType.data.TryGetValue("cellTemplate", out string cellTemplate))
            {
                tableData.cellTemplate = $"<bg>{cellTemplate}</bg>";
            }

            if (componentType.data.TryGetValue("cellClickable", out string cellClickable))
            {
                tableData.clickableCells = Parse.Bool(cellClickable);
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
                    throw new BSMLException($"value '{value}' not found");
                }

                tableData.data = contents.GetValueAs<IList>();
                tableData.tableView.ReloadData();
            }

            RectTransform rectTransform = (RectTransform)componentType.component.transform;

            switch (tableData.tableView.tableType)
            {
                case TableView.TableType.Vertical:
                    rectTransform.sizeDelta = new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    break;
                case TableView.TableType.Horizontal:
                    rectTransform.sizeDelta = new Vector2(tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40);
                    break;
            }

            LayoutElement layoutElement = componentType.component.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = rectTransform.sizeDelta.y;
            layoutElement.preferredWidth = rectTransform.sizeDelta.x;

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
