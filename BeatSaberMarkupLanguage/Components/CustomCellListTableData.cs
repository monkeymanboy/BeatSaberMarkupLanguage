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
        [SerializeField]
        private string cellTemplate;

        [SerializeField]
        private float cellSizeValue = 8.5f;

        [SerializeField]
        private TableView tableView;

        [SerializeField]
        private bool clickableCells = true;

        public string CellTemplate
        {
            get => cellTemplate;
            set => cellTemplate = value;
        }

        public float CellSizeValue
        {
            get => cellSizeValue;
            set => cellSizeValue = value;
        }

        public TableView TableView
        {
            get => tableView;
            set => tableView = value;
        }

        public bool ClickableCells
        {
            get => clickableCells;
            set => clickableCells = value;
        }

        public IList Data { get; set; } = Array.Empty<object>();

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
            tableCell.ParserParams = BSMLParser.Instance.Parse(cellTemplate, tableCell.gameObject, Data[idx]);
            tableCell.SetupPostParse();
            return tableCell;
        }

        public float CellSize(int idx)
        {
            return cellSizeValue;
        }

        public int NumberOfCells()
        {
            return Data.Count;
        }
    }

    public class CustomCellTableCell : TableCell
    {
        [SerializeField]
        private List<GameObject> selectedTags;

        [SerializeField]
        private List<GameObject> hoveredTags;

        [SerializeField]
        private List<GameObject> neitherTags;

        public BSMLParserParams ParserParams { get; internal set; }

        public IList<GameObject> SelectedTags => selectedTags;

        public IList<GameObject> HoveredTags => selectedTags;

        public IList<GameObject> NeitherTags => selectedTags;

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

            if (ParserParams.Actions.TryGetValue("refresh-visuals", out BSMLAction action))
            {
                action.Invoke(selected, highlighted);
            }
        }

        internal void SetupPostParse()
        {
            selectedTags = ParserParams.GetObjectsWithTag("selected");
            hoveredTags = ParserParams.GetObjectsWithTag("hovered");
            neitherTags = ParserParams.GetObjectsWithTag("un-selected-un-hovered");
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
