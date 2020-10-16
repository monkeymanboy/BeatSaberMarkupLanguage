using HMUI;
using System.Collections.Generic;
using IPA.Utilities;
using System.Linq;
using TMPro;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DropDownListSetting : GenericSetting
    {
        private int index;
        
        public List<object> values;

        public SimpleTextDropdown dropdown;

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

                dropdown.SelectCellWithIdx(index);

                UpdateState();
            }
        }

        public override void Setup()
        {
            dropdown.didSelectCellWithIdxEvent += OnSelectIndex;
            ReceiveValue();
            UpdateChoices();
            gameObject.SetActive(true);
        }

        public void UpdateChoices()
        {
            dropdown.SetTexts(values.Select(x => formatter == null ? x.ToString() : (formatter.Invoke(x) as string)).ToList());
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
            dropdown.GetField<TextMeshProUGUI, SimpleTextDropdown>("_text").text = formatter == null ? Value.ToString() : (formatter.Invoke(Value) as string);
        }
    }
}
