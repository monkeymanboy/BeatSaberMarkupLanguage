using System.Collections.Generic;
using HMUI;
using TMPro;
using UnityEngine;
using LevelPackCell = AnnotatedBeatmapLevelCollectionCell; // This got renamed at a point, but old name is more clear so I'm using that

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomListTableData : MonoBehaviour, TableView.IDataSource
    {
        private const string ReuseIdentifier = "BSMLListTableCell";

        [SerializeField]
        private float cellSizeValue = 8.5f;

        [SerializeField]
        private TableView tableView;

        [SerializeField]
        private bool expandCell = false;

        private LevelListTableCell songListTableCellPrefab;
        private LevelPackCell levelPackTableCellPrefab;
        private SimpleTextTableCell simpleTextTableCellPrefab;
        private ListStyle listStyle = ListStyle.List;

        public enum ListStyle
        {
            List,
            Box,
            Simple,
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

        public bool ExpandCell
        {
            get => expandCell;
            set => expandCell = value;
        }

        public ListStyle Style
        {
            get => listStyle;
            set
            {
                // Sets the default cell size for certain styles
                switch (value)
                {
                    case ListStyle.List:
                        cellSizeValue = 8.5f;
                        break;
                    case ListStyle.Box:
                        cellSizeValue = tableView.tableType == TableView.TableType.Horizontal ? 30f : 35f;
                        break;
                    case ListStyle.Simple:
                        cellSizeValue = 8f;
                        break;
                }

                listStyle = value;
            }
        }

        public IList<CustomCellInfo> Data { get; set; } = new List<CustomCellInfo>();

        public LevelListTableCell GetTableCell()
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
            if (!tableCell)
            {
                if (songListTableCellPrefab == null)
                {
                    songListTableCellPrefab = BeatSaberUI.DiContainer.Resolve<LevelCollectionViewController>().GetComponentInChildren<LevelCollectionTableView>(true)._levelCellPrefab;
                }

                tableCell = Instantiate(songListTableCellPrefab);
            }

            tableCell._notOwned = false;
            tableCell.name = $"BSML{nameof(LevelListTableCell)}";
            tableCell.reuseIdentifier = ReuseIdentifier;
            return tableCell;
        }

        public BSMLBoxTableCell GetBoxTableCell()
        {
            BSMLBoxTableCell tableCell = (BSMLBoxTableCell)tableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
            if (!tableCell)
            {
                if (levelPackTableCellPrefab == null)
                {
                    levelPackTableCellPrefab = BeatSaberUI.DiContainer.Resolve<AnnotatedBeatmapLevelCollectionsViewController>().GetComponentInChildren<AnnotatedBeatmapLevelCollectionsGridView>(true)._cellPrefab;
                }

                tableCell = InstantiateBoxTableCell(levelPackTableCellPrefab);
            }

            tableCell.name = nameof(BSMLBoxTableCell);
            tableCell.reuseIdentifier = ReuseIdentifier;
            return tableCell;
        }

        public BSMLBoxTableCell InstantiateBoxTableCell(LevelPackCell levelPackTableCell)
        {
            levelPackTableCell = Instantiate(levelPackTableCell);
            ImageView coverImage = levelPackTableCell._coverImage;
            ImageView selectionImage = levelPackTableCell._selectionImage;

            // sizeDelta doesn't work properly when cell size changes
            RectTransform selectionRectTransform = selectionImage.rectTransform;
            selectionRectTransform.sizeDelta = Vector2.zero;
            selectionRectTransform.anchorMin = new Vector2(-0.25f, -0.25f);
            selectionRectTransform.anchorMax = new Vector2(1.25f, 1.25f);

            foreach (Transform child in coverImage.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject cellObject = levelPackTableCell.gameObject;
            Destroy(levelPackTableCell);
            BSMLBoxTableCell boxTableCell = cellObject.AddComponent<BSMLBoxTableCell>();
            boxTableCell.SetComponents(coverImage, selectionImage);
            return boxTableCell;
        }

        public SimpleTextTableCell GetSimpleTextTableCell()
        {
            SimpleTextTableCell tableCell = (SimpleTextTableCell)tableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
            if (!tableCell)
            {
                if (simpleTextTableCellPrefab == null)
                {
                    simpleTextTableCellPrefab = BeatSaberUI.DiContainer.Resolve<PlayerOptionsViewController>().GetComponentInChildren<SimpleTextDropdown>(true)._cellPrefab;
                }

                tableCell = Instantiate(simpleTextTableCellPrefab);
            }

            tableCell.name = $"BSML{nameof(SimpleTextTableCell)}";
            tableCell.reuseIdentifier = ReuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell();

                    TextMeshProUGUI nameText = tableCell._songNameText;
                    TextMeshProUGUI authorText = tableCell._songAuthorText;
                    tableCell._songBpmText.gameObject.SetActive(false);
                    tableCell._songDurationText.gameObject.SetActive(false);
                    tableCell._promoBadgeGo.SetActive(false);
                    tableCell._updatedBadgeGo.SetActive(false);
                    tableCell._favoritesBadgeImage.gameObject.SetActive(false);
                    tableCell.transform.Find("BpmIcon").gameObject.SetActive(false);
                    if (expandCell)
                    {
                        nameText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                        authorText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                    }

                    nameText.text = Data[idx].Text;
                    authorText.text = Data[idx].Subtext;
                    tableCell._coverImage.sprite = Data[idx].Icon == null ? Utilities.ImageResources.BlankSprite : Data[idx].Icon;

                    return tableCell;
                case ListStyle.Box:
                    BSMLBoxTableCell cell = GetBoxTableCell();
                    cell.SetData(Data[idx].Icon == null ? Utilities.ImageResources.BlankSprite : Data[idx].Icon);

                    return cell;
                case ListStyle.Simple:
                    SimpleTextTableCell simpleCell = GetSimpleTextTableCell();
                    simpleCell._text.richText = true;
                    simpleCell._text.enableWordWrapping = true;
                    simpleCell.text = Data[idx].Text;

                    return simpleCell;
            }

            return null;
        }

        public float CellSize(int idx)
        {
            return cellSizeValue;
        }

        public int NumberOfCells()
        {
            return Data.Count;
        }

        public class CustomCellInfo
        {
            public CustomCellInfo(string text, string subtext = null, Sprite icon = null)
            {
                this.Text = text;
                this.Subtext = subtext;
                this.Icon = icon;
            }

            public string Text { get; }

            public string Subtext { get; }

            public Sprite Icon { get; }
        }
    }
}
