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
        private LevelListTableCell songListTableCellInstance;
        public TableView tableView;

        public bool expandCell = false;
        public LevelListTableCell GetTableCell(bool beatmapCharacteristicImages = false)
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if(songListTableCellInstance == null) songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));
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

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            LevelListTableCell tableCell = GetTableCell();
            if(expandCell) tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").rectTransform.anchorMax = new Vector3(2, 1, 0);
            tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = data[idx].text;
            tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = data[idx].subtext;
            tableCell.GetPrivateField<RawImage>("_coverRawImage").texture = data[idx].icon == null ? Texture2D.blackTexture : data[idx].icon;

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

        public class CustomCellInfo
        {
            public string text;
            public string subtext;
            public Texture icon;

            public CustomCellInfo(string text, string subtext, Texture icon = null)
            {
                this.text = text;
                this.subtext = subtext;
                this.icon = icon;
            }
        };
    }
}
