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
            CustomListTableData tableData = componentType.Component as CustomListTableData;
            ScrollView scrollView = tableData.TableView._scrollView;

            if (componentType.Data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.TableView.didSelectCellWithIdxEvent += (TableView table, int index) =>
                {
                    if (!parserParams.Actions.TryGetValue(selectCell, out BSMLAction action))
                    {
                        throw new ActionNotFoundException(selectCell, parserParams.Host);
                    }

                    action.Invoke(table, index);
                };
            }

            bool verticalList = true;

            if (componentType.Data.TryGetValue("listDirection", out string listDirection))
            {
                tableData.TableView._tableType = (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection);
                scrollView._scrollViewDirection = (ScrollView.ScrollViewDirection)Enum.Parse(typeof(ScrollView.ScrollViewDirection), listDirection);
                verticalList = listDirection.ToLower() != "horizontal";
            }

            if (componentType.Data.TryGetValue("listStyle", out string listStyle))
            {
                tableData.Style = (ListStyle)Enum.Parse(typeof(ListStyle), listStyle);
            }

            if (componentType.Data.TryGetValue("cellSize", out string cellSize))
            {
                tableData.CellSizeValue = Parse.Float(cellSize);
            }

            if (componentType.Data.TryGetValue("expandCell", out string expandCell))
            {
                tableData.ExpandCell = Parse.Bool(expandCell);
            }

            if (componentType.Data.TryGetValue("alignCenter", out string alignCenter))
            {
                tableData.TableView._alignToCenter = Parse.Bool(alignCenter);
            }

            // We can only show the scroll bar for vertical lists
            if (verticalList && componentType.Data.TryGetValue("showScrollbar", out string showScrollbar) && Parse.Bool(showScrollbar))
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
                scrollBar.SetParent(componentType.Component.transform, false);
                Object.Destroy(textScrollView.gameObject);
            }

            if (componentType.Data.TryGetValue("data", out string value))
            {
                if (!parserParams.Values.TryGetValue(value, out BSMLValue contents))
                {
                    throw new ValueNotFoundException(value, parserParams.Host);
                }

                tableData.Data = contents.GetValueAs<IList<CustomCellInfo>>();
                tableData.TableView.ReloadData();
            }

            Vector2 preferredSize = tableData.TableView.tableType switch
            {
                TableView.TableType.Vertical => new Vector2(componentType.Data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.CellSizeValue * (componentType.Data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7)),
                TableView.TableType.Horizontal => new Vector2(tableData.CellSizeValue * (componentType.Data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.Data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40),
                _ => throw new ArgumentException("Unexpected table type " + tableData.TableView.tableType),
            };

            LayoutElement layoutElement = componentType.Component.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredSize.y;
            layoutElement.preferredWidth = preferredSize.x;

            tableData.TableView.gameObject.SetActive(true);
            tableData.TableView.LazyInit();

            if (componentType.Data.TryGetValue("id", out string id))
            {
                parserParams.AddEvent(id + "#PageUp", scrollView.PageUpButtonPressed);
                parserParams.AddEvent(id + "#PageDown", scrollView.PageDownButtonPressed);
            }
        }
    }
}
