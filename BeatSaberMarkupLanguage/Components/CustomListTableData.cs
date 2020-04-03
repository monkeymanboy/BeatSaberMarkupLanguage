using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LevelPackTableCell = AnnotatedBeatmapLevelCollectionTableCell;//This got renamed at a point, but old name is more clear so I'm using that

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomListTableData : MonoBehaviour, TableView.IDataSource
    {
        public enum ListStyle
        {
            List, Box, Simple
        }

        private ListStyle listStyle = ListStyle.List;

        private LevelListTableCell songListTableCellInstance;
        private LevelPackTableCell levelPackTableCellInstance;
        private MainSettingsTableCell mainSettingsTableCellInstance;

        public List<CustomCellInfo> data = new List<CustomCellInfo>();
        public float cellSize = 8.5f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;

        public bool expandCell = false;

        public ListStyle Style
        {
            get => listStyle;
            set
            {
                //Sets the default cell size for certain styles
                switch (value)
                {
                    case ListStyle.List:
                        cellSize = 8.5f;
                        break;
                    case ListStyle.Box:
                        cellSize = tableView.tableType == TableView.TableType.Horizontal ? 30f : 35f;
                        break;
                    case ListStyle.Simple:
                        cellSize = 8f;
                        break;
                }

                listStyle = value;
            }
        }

        public LevelListTableCell GetTableCell(bool beatmapCharacteristicImages = false)
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (songListTableCellInstance == null)
                    songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                tableCell = Instantiate(songListTableCellInstance);
            }

            if (!beatmapCharacteristicImages)
            {
                foreach (Image i in tableCell.GetField<Image[], LevelListTableCell>("_beatmapCharacteristicImages"))
                    i.enabled = false;
            }
            tableCell.transform.Find("FavoritesIcon").gameObject.SetActive(false);

            tableCell.SetField("_beatmapCharacteristicImages", new Image[0]);
            tableCell.SetField("_bought", true);
            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public LevelPackTableCell GetLevelPackTableCell()
        {
            LevelPackTableCell tableCell = (LevelPackTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (levelPackTableCellInstance == null)
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "AnnotatedBeatmapLevelCollectionTableCell");

                tableCell = Instantiate(levelPackTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public MainSettingsTableCell GetMainSettingsTableCell()
        {
            MainSettingsTableCell tableCell = (MainSettingsTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (mainSettingsTableCellInstance == null)
                    mainSettingsTableCellInstance = Resources.FindObjectsOfTypeAll<MainSettingsTableCell>().First(x => x.name == "MainSettingsTableCell");

                tableCell = Instantiate(mainSettingsTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell();
                    if (expandCell)
                    {
                        tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText").rectTransform.anchorMax = new Vector3(2, 1, 0);
                        tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_authorText").rectTransform.anchorMax = new Vector3(2, 1, 0);
                    }

                    tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText").text = data[idx].text;
                    tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_authorText").text = data[idx].subtext;
                    tableCell.GetField<RawImage, LevelListTableCell>("_coverRawImage").texture = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;

                    return tableCell;
                case ListStyle.Box:
                    LevelPackTableCell cell = GetLevelPackTableCell();
                    cell.showNewRibbon = false;
                    cell.GetField<TextMeshProUGUI, LevelPackTableCell>("_infoText").text = $"{data[idx].text}\n{data[idx].subtext}";
                    Image packCoverImage = cell.GetField<Image, LevelPackTableCell>("_coverImage");

                    Texture2D tex = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;
                    packCoverImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
                    packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp;

                    return cell;
                case ListStyle.Simple:
                    MainSettingsTableCell simpleCell = GetMainSettingsTableCell();
                    simpleCell.settingsSubMenuText = data[idx].text;

                    return simpleCell;
            }

            return null;
        }

        public float CellSize()
        {
            return cellSize;
        }

        public int NumberOfCells()
        {
            return data.Count();
        }

        public class CustomCellInfo
        {
            public string text;
            public string subtext;
            public Texture2D icon;

            public CustomCellInfo(string text, string subtext = null, Texture2D icon = null)
            {
                this.text = text;
                this.subtext = subtext;
                this.icon = icon;
            }
        };
    }
}
