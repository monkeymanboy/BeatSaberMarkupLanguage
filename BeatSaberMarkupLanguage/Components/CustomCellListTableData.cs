using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomCellListTableData : MonoBehaviour, TableView.IDataSource
    {
        public List<object> data = new List<object>();
        public string cellTemplate;
        public float cellSize = 8.5f;
        public TableView tableView;
        public bool clickableCells = true;

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            CustomCellTableCell tableCell = new GameObject().AddComponent<CustomCellTableCell>();
            if (clickableCells)
            {
                tableCell.gameObject.AddComponent<Touchable>();
                tableCell.interactable = true;
            }

            tableCell.reuseIdentifier = "BSMLCustomCellListCell";
            tableCell.name = "BSMLCustomTableCell";
            tableCell.parserParams = BSMLParser.instance.Parse(cellTemplate, tableCell.gameObject, data[idx]);
            tableCell.SetupPostParse();
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
        public List<GameObject> selectedTags;
        public List<GameObject> hoveredTags;
        public List<GameObject> neitherTags;

        internal void SetupPostParse()
        {
            selectedTags = parserParams.GetObjectsWithTag("selected");
            hoveredTags = parserParams.GetObjectsWithTag("hovered");
            neitherTags = parserParams.GetObjectsWithTag("un-selected-un-hovered");
        }

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
            foreach (GameObject gameObject in selectedTags)
            {
                gameObject.SetActive(selected);
            }

            foreach (GameObject gameObject in hoveredTags)
            {
                gameObject.SetActive(highlighted);
            }

            foreach (GameObject gameObject in neitherTags)
            {
                gameObject.SetActive(!(selected || highlighted));
            }

            if (parserParams.actions.TryGetValue("refresh-visuals", out BSMLAction action))
            {
                action.Invoke(selected, highlighted);
            }
        }
    }
}
