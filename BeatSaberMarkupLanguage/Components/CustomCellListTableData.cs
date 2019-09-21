using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomCellListTableData : MonoBehaviour, TableView.IDataSource
    {
        public List<object> data = new List<object>();
        public string cellTemplate;
        public float cellSize = 8.5f;
        public TableView tableView;

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            CustomCellTableCell tableCell = new GameObject().AddComponent<CustomCellTableCell>();
            tableCell.gameObject.AddComponent<Touchable>();
            tableCell.interactable = true;
            tableCell.reuseIdentifier = "BSMLCustomCellListCell";
            tableCell.name = "BSMLCustomTableCell";
            tableCell.parserParams = BSMLParser.instance.Parse(cellTemplate, tableCell.gameObject, data[idx]);
            return tableCell;
        }

        public float CellSize()
        {
            return cellSize;
        }

        public int NumberOfCells()
        {
            return data.Count();
        }
    }
    public class CustomCellTableCell : TableCell
    {
        public BSMLParserParams parserParams;

        protected override void SelectionDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        public virtual void RefreshVisuals()
        {
            if (parserParams.objectsWithID.TryGetValue("cell-selected", out GameObject selected))
            {
                selected.SetActive(base.selected);
            }
            if (parserParams.objectsWithID.TryGetValue("cell-unselected", out GameObject unselected))
            {
                unselected.SetActive(!base.selected);
            }
        }
    }
}
