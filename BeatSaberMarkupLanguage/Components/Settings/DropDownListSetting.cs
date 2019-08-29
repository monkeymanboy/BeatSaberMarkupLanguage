using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using static HMUI.TableView;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    class DropDownListSetting : MonoBehaviour, IDataSource
    {
        public List<object> values;

        public TableView tableView;
        public LabelAndValueDropdownWithTableView dropdown;

        private string reuseIdentifier = "BSMLDropdownSetting";
        private EnvironmentTableCell tableCellInstance;


        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange;

        private int index;
        public object Value
        {
            get
            {
                ValidateRange();
                return values[index];
            }
            set
            {
                index = values.IndexOf(value);
                if (index < 0) index = 0;
                UpdateState();
            }
        }

        public EnvironmentTableCell GetTableCell()
        {
            EnvironmentTableCell tableCell = (EnvironmentTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (tableCellInstance == null)
                    tableCellInstance = Resources.FindObjectsOfTypeAll<EnvironmentTableCell>().First();
                tableCell = Instantiate(tableCellInstance);
            }
            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public TableCell CellForIdx(TableView tableView, int idx)
        {
            EnvironmentTableCell environmentTableCell = GetTableCell();
            environmentTableCell.text = values[idx].ToString();
            return environmentTableCell;
        }

        public float CellSize()
        {
            return 8;
        }

        public int NumberOfCells()
        {
            if (values == null)
                return 0;
            return values.Count();
        }


        public void Setup()
        {
            dropdown.didSelectCellWithIdxEvent += OnSelectIndex;
            ReceiveValue();
        }

        private void OnSelectIndex(DropdownWithTableView tableView, int index)
        {
            this.index = index;
            UpdateState();
            onChange?.Invoke(Value);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }
        public void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Value);
        }
        public void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = associatedValue.GetValue();
                dropdown.SelectCellWithIdx(index);
            }
        }

        private void ValidateRange()
        {
            if (index >= values.Count) index = values.Count - 1;
            if (index < 0) index = 0;
        }

        private void UpdateState()
        {
            dropdown.SetValueText(Value.ToString());
        }
    }
}
