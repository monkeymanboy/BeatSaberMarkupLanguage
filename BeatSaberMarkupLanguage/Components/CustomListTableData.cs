using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomListTableData : MonoBehaviour, TableView.IDataSource
    {
        public List<CustomCellInfo> data = new List<CustomCellInfo>();
        public float cellSize = 8.5f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;
        private ListStyle listStyle = ListStyle.List;
        public ListStyle Style
        {
            get
            {
                return listStyle;
            }
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
                }
                listStyle = value;
            }
        }

        private LevelListTableCell songListTableCellInstance;
        private LevelPackTableCell levelPackTableCellInstance;

        public bool expandCell = false;
        public LevelListTableCell GetTableCell(bool beatmapCharacteristicImages = false)
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if(songListTableCellInstance == null)
                    songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));
                tableCell = Instantiate(songListTableCellInstance);
            }

            if (!beatmapCharacteristicImages)
            {
                foreach (UnityEngine.UI.Image i in tableCell.GetPrivateField<UnityEngine.UI.Image[]>("_beatmapCharacteristicImages"))
                    i.enabled = false;
            }
            tableCell.SetPrivateField("_beatmapCharacteristicAlphas", new float[0]);
            tableCell.SetPrivateField("_beatmapCharacteristicImages", new UnityEngine.UI.Image[0]);
            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }
        public LevelPackTableCell GetLevelPackTableCell()
        {
            LevelPackTableCell levelPackTableCellInstance = (LevelPackTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!levelPackTableCellInstance)
            {
                if (levelPackTableCellInstance == null)
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "LevelPackTableCell");
                levelPackTableCellInstance = Instantiate(levelPackTableCellInstance);
            }
            levelPackTableCellInstance.reuseIdentifier = reuseIdentifier;
            return levelPackTableCellInstance;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell();
                    if (expandCell) tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").rectTransform.anchorMax = new Vector3(2, 1, 0);
                    tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = data[idx].text;
                    tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = data[idx].subtext;
                    tableCell.GetPrivateField<RawImage>("_coverRawImage").texture = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;
                    return tableCell;
                case ListStyle.Box:
                    LevelPackTableCell cell = GetLevelPackTableCell();
                    cell.showNewRibbon = false;
                    cell.GetPrivateField<TextMeshProUGUI>("_packNameText").text = data[idx].text;
                    cell.GetPrivateField<TextMeshProUGUI>("_infoText").text = data[idx].subtext;
                    UnityEngine.UI.Image packCoverImage = cell.GetPrivateField<UnityEngine.UI.Image>("_coverImage");
                    Texture2D tex = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;
                    packCoverImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
                    packCoverImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
                    return cell;
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

            public CustomCellInfo(string text, string subtext, Texture2D icon = null)
            {
                this.text = text;
                this.subtext = subtext;
                this.icon = icon;
            }
        };

        public enum ListStyle
        {
            List, Box
        }
    }
}
