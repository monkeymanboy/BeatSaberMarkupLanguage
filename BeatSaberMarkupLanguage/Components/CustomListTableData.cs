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
        private SimpleTextTableCell simpleTextTableCellInstance;

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

            //tableCell.SetField("_beatmapCharacteristicImages", new Image[0]);
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

        public SimpleTextTableCell GetSimpleTextTableCell()
        {
            SimpleTextTableCell tableCell = (SimpleTextTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (simpleTextTableCellInstance == null)
                    simpleTextTableCellInstance = Resources.FindObjectsOfTypeAll<SimpleTextTableCell>().First(x => x.name == "SimpleTextTableCell");

                tableCell = Instantiate(simpleTextTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell(false); // explicitly specify false to ensure all of the characteristic images
                                                                        // start disabled

                    TextMeshProUGUI nameText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText");
                    TextMeshProUGUI authorText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_authorText");
                    if (expandCell)
                    {
                        nameText.rectTransform.anchorMax = new Vector3(2, 1, 0);
                        authorText.rectTransform.anchorMax = new Vector3(2, 1, 0);
                    }

                    nameText.text = data[idx].text;
                    authorText.text = data[idx].subtext;
                    tableCell.GetField<RawImage, LevelListTableCell>("_coverRawImage").texture = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;

                    float xPos = -1f;
                    Image[] characImages = tableCell.GetField<Image[], LevelListTableCell>("_beatmapCharacteristicImages");
                    IEnumerable<Sprite> characSprites = data[idx].characteristicSprites;
                    if (characSprites != null)
                    {
                        var characSpriteArr = characSprites.ToArray();
                        if (characSpriteArr.Length > characImages.Length)
                        {
                            Logger.log.Warn($"List cell specifies {characSpriteArr.Length} characteristic sprites, where only {characImages.Length} are supported");
                        }

                        foreach (var (sprite, img) in characSprites.Zip(characImages, (s, img) => (s, img)))
                        {
                            img.enabled = true;
                            img.rectTransform.sizeDelta = new Vector2(2.625f, 4.5f);
                            img.rectTransform.anchoredPosition = new Vector2(xPos, 0);
                            xPos -= img.rectTransform.sizeDelta.x + .5f;
                            img.sprite = sprite;
                        }
                    }

                    nameText.rectTransform.offsetMax = new Vector2(xPos, nameText.rectTransform.offsetMax.y);
                    authorText.rectTransform.offsetMax = new Vector2(xPos, authorText.rectTransform.offsetMax.y);

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
                    SimpleTextTableCell simpleCell = GetSimpleTextTableCell();
                    simpleCell.text = data[idx].text;

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
            public IEnumerable<Sprite> characteristicSprites;

            // this exists to maintain binary compatability
            public CustomCellInfo(string text, string subtext, Texture2D icon) : this(text, subtext, icon, null) { }
            public CustomCellInfo(string text, string subtext = null, Texture2D icon = null, 
                IEnumerable<Sprite> characteristicSprites = null)
            {
                this.text = text;
                this.subtext = subtext;
                this.icon = icon;
                this.characteristicSprites = characteristicSprites;
            }
        };
    }
}
