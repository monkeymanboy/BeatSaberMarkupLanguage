using System;
using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomCellListTableData : MonoBehaviour, TableView.IDataSource
    {
        public string CellTemplate;
        public float CellSizeValue = 8.5f;
        public TableView TableView;
        public bool ClickableCells = true;

        public IList Data { get; set; } = Array.Empty<object>();

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            CustomCellTableCell tableCell = new GameObject().AddComponent<CustomCellTableCell>();
            if (ClickableCells)
            {
                tableCell.gameObject.AddComponent<Touchable>();
                tableCell.interactable = true;
            }

            tableCell.reuseIdentifier = "BSMLCustomCellListCell";
            tableCell.name = "BSMLCustomTableCell";
            tableCell.ParserParams = BSMLParser.Instance.Parse(CellTemplate, tableCell.gameObject, Data[idx]);
            tableCell.SetupPostParse();
            return tableCell;
        }

        public float CellSize(int idx)
        {
            return CellSizeValue;
        }

        public int NumberOfCells()
        {
            return Data.Count;
        }
    }

    public class CustomCellTableCell : TableCell
    {
        public BSMLParserParams ParserParams;
        public List<GameObject> SelectedTags;
        public List<GameObject> HoveredTags;
        public List<GameObject> NeitherTags;

        public virtual void RefreshVisuals()
        {
            foreach (GameObject gameObject in SelectedTags)
            {
                gameObject.SetActive(selected);
            }

            foreach (GameObject gameObject in HoveredTags)
            {
                gameObject.SetActive(highlighted);
            }

            foreach (GameObject gameObject in NeitherTags)
            {
                gameObject.SetActive(!(selected || highlighted));
            }

            if (ParserParams.Actions.TryGetValue("refresh-visuals", out BSMLAction action))
            {
                action.Invoke(selected, highlighted);
            }
        }

        internal void SetupPostParse()
        {
            SelectedTags = ParserParams.GetObjectsWithTag("selected");
            HoveredTags = ParserParams.GetObjectsWithTag("hovered");
            NeitherTags = ParserParams.GetObjectsWithTag("un-selected-un-hovered");
        }

        protected override void SelectionDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }
    }
}
