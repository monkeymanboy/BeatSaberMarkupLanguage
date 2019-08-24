using BeatSaberMarkupLanguage.Components;
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
            { "listWidth", new[] { "list-width" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, BSMLAction> actions)
        {
            CustomListTableData tableData = obj as CustomListTableData;
            if (data.ContainsKey("selectCell"))
            {
                tableData.tableView.didSelectCellWithIdxEvent += delegate(HMUI.TableView table, int index) {
                    if (!actions.ContainsKey(data["selectCell"]))
                        throw new Exception("select-cell action '" + data["onClick"] + "' not found");
                    actions[data["selectCell"]].Invoke(table, index);
                };
            }
            if(data.ContainsKey("cellSize"))
                tableData.cellSize = float.Parse(data["cellSize"]);
            RectTransform viewport = tableData.tableView.GetComponentInChildren<ScrollRect>().viewport;
            viewport.sizeDelta = new Vector2(data.ContainsKey("listWidth") ? float.Parse(data["listWidth"]) : 60, tableData.cellSize * (data.ContainsKey("visibleCells") ? float.Parse(data["visibleCells"]) : 8));
            obj.gameObject.GetComponent<LayoutElement>().preferredHeight = viewport.sizeDelta.y;
            obj.gameObject.GetComponent<LayoutElement>().preferredWidth = viewport.sizeDelta.x;
            tableData.tableView.gameObject.SetActive(true);
            if (data.ContainsKey("id"))
            {
                TableViewScroller scroller = tableData.tableView.GetPrivateField<TableViewScroller>("_scroller");
                actions.Add(data["id"] + "#PageUp", new BSMLAction(scroller, scroller.GetType().GetMethod("PageScrollDown", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
                actions.Add(data["id"] + "#PageDown", new BSMLAction(scroller, scroller.GetType().GetMethod("PageScrollUp", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
            }
        }
    }
}
