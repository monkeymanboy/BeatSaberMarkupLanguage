using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HMUI.TableView;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DropDownListSetting : GenericSetting, IDataSource
    {
        private string reuseIdentifier = "BSMLDropdownSetting";
        private EnvironmentTableCell tableCellInstance;

        private int index;
        
        public List<object> values;

        public TableView tableView;
        public LabelAndValueDropdownWithTableView dropdown;

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
                if (index < 0)
                    index = 0;

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
            environmentTableCell.text = formatter == null ? values[idx].ToString() : (formatter.Invoke(values[idx]) as string);
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
            else
                return values.Count();
        }

        public override void Setup()
        {
            dropdown.didSelectCellWithIdxEvent += OnSelectIndex;
            ReceiveValue();
            dropdown.ReloadData();
            gameObject.SetActive(true);
        }

        private void OnSelectIndex(DropdownWithTableView tableView, int index)
        {
            this.index = index;
            UpdateState();
            onChange?.Invoke(Value);
            if (updateOnChange)
                ApplyValue();
        }

        public override void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = associatedValue.GetValue();
                dropdown.SelectCellWithIdx(index);
            }
        }

        private void ValidateRange()
        {
            if (index >= values.Count)
                index = values.Count - 1;

            if (index < 0)
                index = 0;
        }

        private void UpdateState()
        {
            dropdown.SetValueText(formatter == null ? Value.ToString() : (formatter.Invoke(Value) as string));
        }
    }
}
