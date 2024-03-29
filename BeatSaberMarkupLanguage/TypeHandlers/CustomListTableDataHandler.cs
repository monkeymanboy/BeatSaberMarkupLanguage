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
                        throw new ActionNotFoundException(selectCell, parserParams.host);
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
            if (verticalList && componentType.data.TryGetValue("showScrollbar", out string showScrollbar) && Parse.Bool(showScrollbar))
            {
                TextPageScrollView textScrollView = Object.Instantiate(ScrollViewTag.ScrollViewTemplate);

                Button pageUpButton = textScrollView._pageUpButton;
                Button pageDownButton = textScrollView._pageDownButton;
                VerticalScrollIndicator verticalScrollIndicator = textScrollView._verticalScrollIndicator;
                RectTransform scrollBar = (RectTransform)verticalScrollIndicator.transform.parent;

                scrollBar.offsetMin = Vector2.zero;
                scrollBar.offsetMax = new Vector2(8, 0);

                scrollView._pageUpButton = pageUpButton;
                scrollView._pageDownButton = pageDownButton;
                scrollView._verticalScrollIndicator = verticalScrollIndicator;
                scrollBar.SetParent(componentType.component.transform, false);
                Object.Destroy(textScrollView.gameObject);
            }

            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new ValueNotFoundException(value, parserParams.host);
                }

                tableData.data = contents.GetValueAs<IList<CustomCellInfo>>();
                tableData.tableView.ReloadData();
            }

            Vector2 preferredSize = tableData.tableView.tableType switch
            {
                TableView.TableType.Vertical => new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7)),
                TableView.TableType.Horizontal => new Vector2(tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40),
                _ => throw new ArgumentException("Unexpected table type " + tableData.tableView.tableType),
            };

            LayoutElement layoutElement = componentType.component.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredSize.y;
            layoutElement.preferredWidth = preferredSize.x;

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
