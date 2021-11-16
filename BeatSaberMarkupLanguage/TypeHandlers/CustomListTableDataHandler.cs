using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        // We need to use hard reflection like this because ScrollView's direction enum is private.
        private readonly FieldInfo scrollViewDirectionField = typeof(ScrollView)
             .GetField("_scrollViewDirection", BindingFlags.Instance | BindingFlags.NonPublic);

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
            { "alignCenter", new[] { "align-to-center" } },
            { "stickScrolling", new[] { "stick-scrolling" } },
            { "showScrollbar", new[] { "show-scrollbar" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            CustomListTableData tableData = componentType.component as CustomListTableData;
            ScrollView scrollView = tableData.tableView.GetField<ScrollView, TableView>("_scrollView");

            if (componentType.data.TryGetValue("selectCell", out string selectCell))
            {
                tableData.tableView.didSelectCellWithIdxEvent += delegate (TableView table, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
                        throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");

                    action.Invoke(table, index);
                };
            }

            bool verticalList = true;

            if (componentType.data.TryGetValue("listDirection", out string listDirection))
            {
                tableData.tableView.SetField<TableView, TableType>("_tableType", (TableType)Enum.Parse(typeof(TableType), listDirection));
                scrollViewDirectionField.SetValue(scrollView, Enum.Parse(scrollViewDirectionField.FieldType, listDirection));
                verticalList = listDirection.ToLower() != "horizontal";
            }

            if (componentType.data.TryGetValue("listStyle", out string listStyle))
                tableData.Style = (ListStyle)Enum.Parse(typeof(ListStyle), listStyle);

            if (componentType.data.TryGetValue("cellSize", out string cellSize))
                tableData.cellSize = Parse.Float(cellSize);

            if (componentType.data.TryGetValue("expandCell", out string expandCell))
                tableData.expandCell = Parse.Bool(expandCell);

            if (componentType.data.TryGetValue("alignCenter", out string alignCenter))
                tableData.tableView.SetField<TableView, bool>("_alignToCenter", Parse.Bool(alignCenter));

            if (componentType.data.TryGetValue("stickScrolling", out string stickScrolling))
            {
                if (Parse.Bool(stickScrolling))
                    scrollView.SetField("_platformHelper", BeatSaberUI.PlatformHelper);
            }

            // We can only show the scroll bar for vertical lists
            if (verticalList && componentType.data.TryGetValue("showScrollbar", out string showScrollbar))
            {
                if (Parse.Bool(showScrollbar))
                {
                    TextPageScrollView textScrollView = UnityEngine.Object.Instantiate(ScrollViewTag.ScrollViewTemplate, componentType.component.transform);

                    Button pageUpButton = textScrollView.GetField<Button, ScrollView>("_pageUpButton");
                    Button pageDownButton = textScrollView.GetField<Button, ScrollView>("_pageDownButton");
                    VerticalScrollIndicator verticalScrollIndicator = textScrollView.GetField<VerticalScrollIndicator, ScrollView>("_verticalScrollIndicator");
                    RectTransform scrollBar = verticalScrollIndicator.transform.parent as RectTransform;

                    scrollView.SetField("_pageUpButton", pageUpButton);
                    scrollView.SetField("_pageDownButton", pageDownButton);
                    scrollView.SetField("_verticalScrollIndicator", verticalScrollIndicator);
                    scrollBar.SetParent(componentType.component.transform);
                    GameObject.Destroy(textScrollView.gameObject);

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
                    throw new Exception("value '" + value + "' not found");
                tableData.data = contents.GetValue() as List<CustomCellInfo>;
                tableData.tableView.ReloadData();
            }

            switch (tableData.tableView.tableType)
            {
                case TableType.Vertical:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    break;
                case TableType.Horizontal:
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
